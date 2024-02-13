using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWP : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] waypoints;
    private int currentWP = 0;
    public float speed = 10.0f;
    public float rotSpeed = 10.0f;
    public float distanceFromWp = 10.0f;
    public float lookAhead = 10.0f;
    GameObject tracker;
    void Start()
    {
        tracker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        DestroyImmediate(tracker.GetComponent<Collider>());

        tracker.GetComponent<Renderer>().enabled = false;
        
        tracker.transform.position = transform.position;
        tracker.transform.rotation = transform.rotation;
    }

    void ProgressTracker()
    {
        if (Vector3.Distance(tracker.transform.position, transform.position) > lookAhead) return;

        if (Vector3.Distance(tracker.transform.position, waypoints[currentWP].transform.position) < distanceFromWp)
        {
            currentWP++;
            if (currentWP >= waypoints.Length)
            {
                currentWP = 0;
            }
        }

        tracker.transform.LookAt(waypoints[currentWP].transform.position);
        tracker.transform.Translate(0, 0, (speed + lookAhead) * Time.deltaTime);
    }

    void Update()
    {
        ProgressTracker();

        Quaternion lookAtWp = Quaternion.LookRotation(waypoints[currentWP].transform.position - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtWp, rotSpeed * Time.deltaTime);
        transform.Translate(0, 0, speed * Time.deltaTime);


    }
}