using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCylinder : MonoBehaviour {

    public GameObject obstacle;
    GameObject[] agents;

    void Start() {

        agents = GameObject.FindGameObjectsWithTag("agent");  // Tag for agents
    }


    void Update() {

        if (Input.GetMouseButtonDown(0)) {                                          // If we have clicked in our scene then instantiate a cylinder.

            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);            // Create a raycast in the scene from the main camera view,
                                                                                    //from click position into environment.
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo)) {   

                Instantiate(obstacle, hitInfo.point, obstacle.transform.rotation);  // If hit then instantiate the flee object.
                foreach (GameObject a in agents) {                                  // Sent info to agents that cylinder has been dropped.

                    a.GetComponent<AIControl>().DetectNewObstacle(hitInfo.point);
                }
            }
        }
    }
}

// Drom this script onto the camera object. It can actually be dropped anywhwere so long as it is in the scene.