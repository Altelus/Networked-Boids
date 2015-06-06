/*******************************************************************************
Filename:   Flock.cs
Author:     Geoffrey Mok (100515125)
Date:       April 09, 2015
Purpose:    Contains all functionality relating to a single flock player, manages
 *          the collection of boids and applies the boid rules of cohesion, alignment
 *          and separation
 *          Attached boids focus on following player(flock leader)
 *          Flock leader is represented by a star/light
*******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flock : Photon.MonoBehaviour
{
    public GameObject boidPrefab;
    public Color flockColor;
    public GameObject attachedLight;
    public Camera cam;

    // Flock settings
    public Vector3 minBounds = new Vector3(-50, 0, -50);
    public Vector3 maxBounds = new Vector3(50, 100, 50);
    public int flockSize = 10;
    public float leaderSpeed = 100f;
    public float maxVelocity = 5f;
    public float cohesionFactor = 100f;
    public float alignmentFactor = 8f;
    public float seperationDistance = 10f;
    public float targetFocusFactor = 100f;

    // Flags
    public bool isActive = true;
    public bool isTrailRendererActive = false;

    // The collection of boids
    public GameObject[] boids;

    // Dead-Reckoning
    public bool isDeadReckoning = true;
    private float prevSyncTime = 0f;
    private float latency = 0f;
    private float syncTime = 0f;
    private Vector3 otherFlockLeaderPrevPosition = Vector3.zero;
    private Vector3 otherFlockLeaderNewPosition = Vector3.zero;

    private Vector3[] otherFlockBoidPrevPosition;
    private Vector3[] otherFlockBoidNewPosition;

    void Awake()
    {
        //Debug.Log("AWAKE");

        // Initialize boids
        boids = new GameObject[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            boids[i] = Instantiate(boidPrefab, transform.position, transform.rotation) as GameObject;
            boids[i].GetComponent<Boid>().minBounds = minBounds;
            boids[i].GetComponent<Boid>().maxBounds = maxBounds;
        }

        SetFlockColor(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        // Tag for other scripts to find
        if (photonView.isMine)
            tag = "LocalFlock";
        else
            tag = "RemoteFlock";

        otherFlockBoidPrevPosition = new Vector3[flockSize];
        otherFlockBoidNewPosition = new Vector3[flockSize];
    }

    void Update()
    {
        // if current flock object is spawned by me (Local flock), allow for user input
        if (photonView.isMine) 
        {
            //// Toggle pausing of flock (for debugging)
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    SetFlockStatus(!isActive);
            //}
            // Randomize Color
            if (Input.GetKeyDown(KeyCode.R))
            {
                SetFlockColor(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            }
            // Toggle trail renderer
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleTrailRenderer(!isTrailRendererActive);
            }

            if (isActive)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    GetComponent<Rigidbody>().velocity += transform.forward * leaderSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    GetComponent<Rigidbody>().velocity -= transform.forward * leaderSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    GetComponent<Rigidbody>().velocity += transform.right * leaderSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    GetComponent<Rigidbody>().velocity -= transform.right * leaderSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    GetComponent<Rigidbody>().velocity += transform.up * leaderSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    GetComponent<Rigidbody>().velocity -= transform.up * leaderSpeed * Time.deltaTime;
                }

                UpdateBoids();
            }
        }
        // if current flock object is not spawned by me (Remote Flock), just update
        else
        {
            if (isActive)
            {
                // Interpolation of flock leader position
                syncTime += Time.deltaTime;
                GetComponent<Rigidbody>().position = Vector3.Lerp(otherFlockLeaderPrevPosition, otherFlockLeaderNewPosition, syncTime / latency);

                // Interpolation of boids
                for (int i = 0; i < boids.Length; i++)
                {
                    if (isDeadReckoning)
                    {
                        boids[i].transform.position = Vector3.Lerp(otherFlockBoidPrevPosition[i], otherFlockBoidNewPosition[i], syncTime / latency);
                    }
                    else
                        boids[i].transform.position = otherFlockBoidNewPosition[i];

                }
            }
        }
    }

    void UpdateBoids()
    {
        float totalFlockSize = flockSize;

        // Pre-calculate sum of positions and velocities
        Vector3 sumPos = Vector3.zero;
        Vector3 sumVel = Vector3.zero;
        foreach (GameObject localBoid in boids)
        {
            sumPos += localBoid.transform.position;
            sumVel += localBoid.GetComponent<Boid>().velocity;
        }

        // account for remote flock
        GameObject remoteFlock = GameObject.FindWithTag("RemoteFlock");
        if (remoteFlock != null)
        {
            foreach (GameObject remoteBoid in remoteFlock.GetComponent<Flock>().boids)
            {
                sumPos += remoteBoid.transform.position;
                sumVel += remoteBoid.GetComponent<Boid>().velocity;
            }

            totalFlockSize += remoteFlock.GetComponent<Flock>().boids.Length;
        }

        // Apply boid rules
        foreach (GameObject localBoid in boids)
        {
            //Debug.Log("1: " + boid.GetComponent<Boid>().velocity);

            // Cohesion
            Vector3 cohere = sumPos - localBoid.transform.position;
            cohere = cohere / (totalFlockSize - 1);
            cohere = (cohere - localBoid.transform.position) * cohesionFactor;

            // Alignment
            Vector3 align = sumPos - localBoid.transform.position;
            align = align / (totalFlockSize - 1);
            align = (align - localBoid.GetComponent<Boid>().velocity) * alignmentFactor;

            // Separation
            Vector3 seperate = Vector3.zero;
            foreach (GameObject otherBoid in boids)
            {
                if (localBoid != otherBoid)
                {
                    Vector3 distance = otherBoid.transform.position - localBoid.transform.position;
                    if (distance.magnitude < seperationDistance)
                    {
                        seperate -= distance;
                    }
                }
            }
            if (remoteFlock != null)
            {
                foreach (GameObject remoteBoid in boids)
                {
                    if (localBoid != remoteBoid)
                    {
                        Vector3 distance = remoteBoid.transform.position - localBoid.transform.position;
                        if (distance.magnitude < seperationDistance)
                        {
                            seperate -= distance;
                        }
                    }
                }
            }

            // Target focus on flock leader
            Vector3 target = (transform.position - localBoid.transform.position) * targetFocusFactor;

            localBoid.GetComponent<Boid>().velocity += cohere + align + seperate + target;
            localBoid.GetComponent<Boid>().ApplyVelocityConstraint(maxVelocity);
            //boid.GetComponent<Boid>().ApplyWorldConstrainsts();
            localBoid.transform.position +=  localBoid.GetComponent<Boid>().velocity;
        }
    }

    // Called on network update of both send/receive 
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // sending flock data
        if (stream.isWriting)
        {
            // flock leader
            stream.SendNext(GetComponent<Rigidbody>().position);
            stream.SendNext(GetComponent<Rigidbody>().velocity);

            // flock
            for (int i = 0; i < boids.Length; i++)
            {
                stream.SendNext(boids[i].GetComponent<Boid>().transform.position);
                stream.SendNext(boids[i].GetComponent<Boid>().velocity);
            }
        }
        // receiving flock data
        else
        {
            Vector3 flockLeaderPosition = (Vector3)stream.ReceiveNext();
            Vector3 flockLeaderVelocity = (Vector3)stream.ReceiveNext();

            for (int i = 0; i < boids.Length; i++)
            {
                otherFlockBoidPrevPosition[i] = boids[i].GetComponent<Boid>().transform.position;

                boids[i].GetComponent<Boid>().transform.position = (Vector3)stream.ReceiveNext();
                boids[i].GetComponent<Boid>().velocity = (Vector3)stream.ReceiveNext();

                otherFlockBoidNewPosition[i] = boids[i].GetComponent<Boid>().transform.position;
            }

            // Dead-reckoning attributes for flock leader
            syncTime = 0f;
            latency = Time.time - prevSyncTime;
            prevSyncTime = Time.time;

            otherFlockLeaderNewPosition = flockLeaderPosition + flockLeaderVelocity * latency;
            otherFlockLeaderPrevPosition = GetComponent<Rigidbody>().position;

            //transform.position = flockLeaderPosition;
        }
    }

    // Remote procedure call to change color
    [RPC]
    public void SetFlockColor(float r, float g, float b)
    {
        Debug.Log("SETFLOCKCOLOR");
        flockColor = new Color(r, b, g);

        // Colors of mesh and lights
        GetComponent<Renderer>().material.color = flockColor;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", flockColor);
        attachedLight.GetComponent<Light>().color = flockColor;
        attachedLight.GetComponent<LensFlare>().color = flockColor;

        // Colors of boids
        foreach (GameObject boid in boids)
        {
            boid.GetComponent<MeshRenderer>().material.color = flockColor;
            boid.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", flockColor);
            boid.GetComponent<TrailRenderer>().material.SetColor("_TintColor", flockColor);
        }

        if (photonView.isMine)
        {
            photonView.RPC("SetFlockColor", PhotonTargets.OthersBuffered, r, g, b);
        }

    }

    // Remote procedure call to toggle pausing of boid
    [RPC]
    public void SetFlockStatus(bool status)
    {
        isActive = status;

        if (photonView.isMine)
        {
            photonView.RPC("SetFlockStatus", PhotonTargets.OthersBuffered, status);
        }

    }

    // Remote procedure call to toggle pausing of boid
    [RPC]
    public void ToggleTrailRenderer(bool status)
    {
        isTrailRendererActive = status;

        foreach (GameObject boid in boids)
        {
            boid.GetComponent<TrailRenderer>().enabled = isTrailRendererActive;
        }

        if (photonView.isMine)
        {
            photonView.RPC("ToggleTrailRenderer", PhotonTargets.OthersBuffered, isTrailRendererActive);
        }

    }
}
