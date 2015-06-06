using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {
    public GameObject target;
    public float damping = 1;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.transform.position;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.transform.position + offset;
        Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
        transform.position = position;
        transform.LookAt(target.transform.position);

        //Vector3 targetDir = target.transform.position - transform.position;
        //float step = 1 * Time.deltaTime;
        //Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        //Debug.DrawRay(transform.position, newDir *100, Color.red);
        //transform.rotation = Quaternion.LookRotation(newDir);
    }
}
