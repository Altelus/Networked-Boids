/*******************************************************************************
Filename:   Boid.cs
Author:     Geoffrey Mok (100515125)
Date:       April 09, 2015
Purpose:    Script representing a single boid object, responsible for its velocity
 *          cap and collision avoidance
*******************************************************************************/
using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour {

    public Vector3 minBounds = new Vector3(-50, 0, -50);
    public Vector3 maxBounds = new Vector3(50, 100, 50);

    public Vector3 velocity;
    public float boundingVelocity = 1.2f;

	void Start () {
        SetRandomPos();
        velocity = Vector3.zero;
        ApplyWorldConstrainsts();
	}
	
	void Update () {
	}

    void SetRandomPos()
    {
        GetComponent<Rigidbody>().position = new Vector3(Random.Range(minBounds.x, maxBounds.x),
                                         Random.Range(minBounds.y, maxBounds.y),
                                         Random.Range(minBounds.z, maxBounds.z));
    }

    // Cap boid to bounding box currently UNUSED
    public void ApplyWorldConstrainsts()
    {
        if (GetComponent<Rigidbody>().position.x > maxBounds.x)
            velocity.x += -boundingVelocity;
        else if (GetComponent<Rigidbody>().position.x < minBounds.x)
            velocity.x += boundingVelocity;

        if (GetComponent<Rigidbody>().position.y > maxBounds.y)
            velocity.y += -boundingVelocity;
        else if (GetComponent<Rigidbody>().position.y < minBounds.y)
            velocity.y += boundingVelocity;

        if (GetComponent<Rigidbody>().position.z > maxBounds.z)
            velocity.z += -boundingVelocity;
        else if (GetComponent<Rigidbody>().position.z < minBounds.z)
            velocity.z += boundingVelocity;
    }

    public void ApplyVelocityConstraint(float maxVelocity)
    {
        if (velocity.magnitude > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }
    }

    // Collision avoidance
    void OnTriggerStay(Collider other)
    {
        //Debug.Log("Collision");

        // Move away from collider
        Vector3 direction = GetComponent<Rigidbody>().position - other.transform.position;
        direction.Normalize();
        velocity += direction * 100;
    }
}
