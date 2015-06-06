/*******************************************************************************
Filename:   NetworkManager.cs
Author:     Geoffrey Mok (100515125)
Date:       April 09, 2015
Purpose:    Script attached to the level master NetworkManager gameobject. Responsible
 *          for setting up the photon network  functions
*******************************************************************************/
using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    public GameObject flockPrefab;
    public Transform spawnPoint;

    private GameObject myFlock;
    private GameObject remoteFlock;

    private const string roomName = "100515125_INFR3830_AS2";
    private RoomInfo[] roomsList;

    private float simLatency = 0;

    void Start()
    {
        // Connect to photon master server
        PhotonNetwork.ConnectUsingSettings("0.2");
    }

    void OnGUI()
    {
        // Display connection status
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        // Show room functions
        else if (PhotonNetwork.room == null)
        {
            // Create Room
            if (GUI.Button(new Rect(100, 100, 250, 100), "Create Room"))
            {
                PhotonNetwork.CreateRoom(roomName, true, true, 2);
                //PhotonNetwork.CreateRoom(roomName + System.Guid.NewGuid().ToString("N"), true, true, 2);
            }

            // Join Room
            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
                {
                    if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join Room: " + roomsList[i].name))
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                }
            }
        }
        else
        {
            // Player in Room, Draw HUD

            GUI.Label(new Rect(10, 0, 200, 25), "Geoffrey Mok (100515125)");
            GUI.Label(new Rect(Screen.width - 100, 0, 150, 25), "Latency: " + PhotonNetwork.GetPing().ToString() + "ms");

            if (GUI.Button(new Rect(30, Screen.height - 150, 140, 30), "Randomize Color"))
            {
                myFlock.GetComponent<Flock>().SetFlockColor(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            }

            string state = "OFF";

            // Active Button
            if (myFlock.GetComponent<Flock>().isActive)
                state = "OFF";
            else
                state = "ON";
            if (GUI.Button(new Rect(30, Screen.height - 100, 140, 30), "Toggle Flock " + state))
            {
                myFlock.GetComponent<Flock>().SetFlockStatus(!myFlock.GetComponent<Flock>().isActive);
            }

            // Trail renderer Button
            if (myFlock.GetComponent<Flock>().isTrailRendererActive)
                state = "OFF";
            else
                state = "ON";
            if (GUI.Button(new Rect(30, Screen.height - 50, 140, 30), "Toggle Trail " + state))
            {
                myFlock.GetComponent<Flock>().ToggleTrailRenderer(!myFlock.GetComponent<Flock>().isTrailRendererActive);
            }

            // Flock Controls
            GUI.Label(new Rect(Screen.width - 155, Screen.height - 200, 150, 50), "Max Boid Velocity");
            myFlock.GetComponent<Flock>().maxVelocity = GUI.HorizontalScrollbar(new Rect(Screen.width - 150, Screen.height - 180, 100, 50), myFlock.GetComponent<Flock>().maxVelocity, 0.01f, 0, 10.0f);

            GUI.Label(new Rect(Screen.width - 140, Screen.height - 150, 100, 50), "Cohesion" );
            myFlock.GetComponent<Flock>().cohesionFactor = GUI.HorizontalScrollbar(new Rect(Screen.width - 150, Screen.height - 130, 100, 50), myFlock.GetComponent<Flock>().cohesionFactor, 0.01f, -0.01f, 0.1f);

            GUI.Label(new Rect(Screen.width - 140, Screen.height - 100, 100, 50), "Alignment");
            myFlock.GetComponent<Flock>().alignmentFactor = GUI.HorizontalScrollbar(new Rect(Screen.width - 150, Screen.height - 80, 100, 50), myFlock.GetComponent<Flock>().alignmentFactor, 0.01f, -0.01f, 0.1f);

            GUI.Label(new Rect(Screen.width - 140, Screen.height - 50, 100, 50), "Separation");
            myFlock.GetComponent<Flock>().seperationDistance = GUI.HorizontalScrollbar(new Rect(Screen.width - 150, Screen.height - 30, 100, 50), myFlock.GetComponent<Flock>().seperationDistance, 0.01f, 1, 100);

            // Simulated Latency Controls
            GUI.Label(new Rect(Screen.width - 155, Screen.height - 400, 150, 50), "Simulated Latency");
            simLatency = GUI.HorizontalScrollbar(new Rect(Screen.width - 150, Screen.height - 380, 100, 50), simLatency, 0.01f, 0, 1000f);

            if (simLatency > 0)
            {
                PhotonNetwork.networkingPeer.IsSimulationEnabled = true;
                PhotonNetwork.networkingPeer.NetworkSimulationSettings.IncomingLag = (int)simLatency;
                PhotonNetwork.networkingPeer.NetworkSimulationSettings.OutgoingLag = (int)simLatency;
                PhotonNetwork.networkingPeer.NetworkSimulationSettings.IncomingJitter = (int)simLatency / 4;
                PhotonNetwork.networkingPeer.NetworkSimulationSettings.OutgoingJitter = (int)simLatency / 4;

            }
            else
            {
                PhotonNetwork.networkingPeer.IsSimulationEnabled = false;
            }

            // Dead reckoning toggle
            if (remoteFlock != null)
            {
                if (remoteFlock.GetComponent<Flock>().isDeadReckoning)
                    state = "OFF";
                else
                    state = "ON";

                if (GUI.Button(new Rect(20, Screen.height - 200, 180, 30), "Toggle dead Reckoning " + state))
                {
                    remoteFlock.GetComponent<Flock>().isDeadReckoning = !remoteFlock.GetComponent<Flock>().isDeadReckoning;
                }
            }
        }
    }

    // Called when a list of rooms is received from server
    void OnReceivedRoomListUpdate()
    {
        //Debug.Log("OnReceivedRoomListUpdate");
        roomsList = PhotonNetwork.GetRoomList();
    }

    // Called when current player joins room
    void OnJoinedRoom()
    {
        //Debug.Log("OnJoinedRoom");

        // Spawn player
        myFlock = PhotonNetwork.Instantiate(flockPrefab.name, spawnPoint.position, Quaternion.identity, 0);
        GetComponent<LookAtCamera>().target = myFlock;

        myFlock.GetComponent<Flock>().cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (remoteFlock == null)
            remoteFlock = GameObject.FindWithTag("RemoteFlock");
    }
}
