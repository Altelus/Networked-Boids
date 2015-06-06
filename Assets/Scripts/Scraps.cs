using UnityEngine;
using System.Collections;

public class Scraps : MonoBehaviour {

    //GUI.Box(new Rect(0, 0, 100, 50), "Top-left");
    //GUI.Box(new Rect(Screen.width - 100, 0, 100, 50), "Top-right");
    //GUI.Box(new Rect(0, Screen.height - 50, 100, 50), "Bottom-left");
    //GUI.Box(new Rect(Screen.width - 100, Screen.height - 50, 100, 50), "Bottom right");

    //if (myFlock != null)
    //{
    //    myFlock.GetComponent<Flock>().NetworkedUpdate();
    //}
    //remoteFlock = GameObject.FindGameObjectWithTag("RemoteFlock");


    //remoteFlock.GetComponent<Flock>().NetworkedUpdate();
    //myFlock.GetComponent<Flock>().SetFlockColor(new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));


    //boid.GetComponent<Boid>().velocity = new Vector3(maxVelocity, maxVelocity, maxVelocity);
    //velocity.Scale(new Vector3(maxVelocity / velocity.magnitude, maxVelocity / velocity.magnitude, maxVelocity / velocity.magnitude));


    //other.gameObject.GetComponent<Boid>().velocity += direction * 100;


    //Vector3 v3Pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100);
    //GetComponent<Rigidbody>().position = Camera.main.ScreenToWorldPoint(v3Pos);

    //Vector3 fwd = transform.TransformDirection(Vector3.forward);
    //if (Physics.Raycast(transform.position, fwd, 10))
    //    print("There is something in front of the object!");

    //float horizontal = Input.GetAxis("Horizontal") * 60 * Time.deltaTime;
    //GetComponent<Rigidbody>().transform.Rotate(0, horizontal, 0);

    //float vertical = Input.GetAxis("Vertical") * 100 * Time.deltaTime;
    //GetComponent<Rigidbody>().transform.Translate(0, 0, vertical);


    //RaycastHit vHit = new RaycastHit();
    //Ray vRay = cam.ScreenPointToRay(Input.mousePosition);
    //if (Physics.Raycast(vRay, out vHit, 1000))
    //{
    //    GetComponent<Rigidbody>().position = vHit.point;
    //    Debug.Log("OK");
    //    //Screen.showCursor = false;
    //}



    /*
    Vector3 GetBoidCohesionVector(GameObject currentBoid)
    {
        Vector3 results = Vector3.zero;

        foreach (GameObject boid in boids)
        {
            if (currentBoid != boid)
            {
                results += boid.transform.position;
            }
        }

        results = results / (flockSize - 1);

        return (results - currentBoid.transform.position) / cohesionFactor;
    }

    Vector3 GetBoidAlignmentVector(GameObject currentBoid)
    {
        Vector3 results = Vector3.zero;

        foreach (GameObject boid in boids)
        {
            if (currentBoid != boid)
            {
                results += boid.GetComponent<Boid>().velocity;
            }
        }

        results = results / (flockSize - 1);

        return (results - currentBoid.GetComponent<Boid>().velocity) / alignmentFactor;
    }
    Vector3 GetBoidSeperationVector(GameObject currentBoid)
    {
        Vector3 target = Vector3.zero;

        foreach (GameObject boid in boids)
        {
            if (currentBoid != boid)
            {
                Vector3 distance = boid.transform.position - currentBoid.transform.position;
                if (distance.magnitude < seperationDistance)
                {
                    target -= distance;
                }
            }
        }

        return target;
    }

    Vector3 GetBoidTargetVector(GameObject currentBoid)
    {
        return (transform.position - currentBoid.transform.position) / targetFocusFactor;
    }

    */
}
