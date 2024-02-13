using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour {                                                // Make sure all agents are tagged with the 'agent' tag.

    GameObject[] goalLocations;
    NavMeshAgent agent;
    Animator anim;
    float speedMult;
    float detectionRadius = 20.0f;                                                      // Distance from which agent will detect the cylinder.
    float fleeRadius = 10.0f;                                                           // Distance that agent will flee from the cylinder.

    void Start() {

        agent = GetComponent<NavMeshAgent>();
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        int i = Random.Range(0, goalLocations.Length);
        agent.SetDestination(goalLocations[i].transform.position);
        anim = this.GetComponent<Animator>();
        anim.SetFloat("wOffset", Random.Range(0.0f, 1.0f));
        ResetAgent();
    }

    void ResetAgent() {                                                                 // Reset method that runs every time an agent gets to a gola location.

        speedMult = Random.Range(0.1f, 1.5f);
        anim.SetFloat("speedMult", speedMult);
        agent.speed *= speedMult;
        anim.SetTrigger("isWalking");
        agent.angularSpeed = 120.0f;                                                    // Slower turn speed when walking.
        agent.ResetPath();
    }

    public void DetectNewObstacle(Vector3 position) {                                   // Detects cylinder.

        if (Vector3.Distance(position, this.transform.position) < detectionRadius) {

            Vector3 fleeDirection = (this.transform.position - position).normalized;    // Reverse direction of agent
            Vector3 newGoal = this.transform.position + fleeDirection * fleeRadius;     // and move toward new goal AND for a distance of flee radius.

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newGoal, path);

            if (path.status != NavMeshPathStatus.PathInvalid) {                         // We need to determine if the new path is valid (and not off the NavMesh).

                agent.SetDestination(path.corners[path.corners.Length - 1]);            // Insert path.corners at the end of the NavMesh list of points.
                anim.SetTrigger("isRunning");
                agent.speed = 10.0f;
                agent.angularSpeed = 500.0f;                                            // How fast agent turn arounds when fleeing (running away). 
            }
        }
    }

    void Update() {

        if (agent.remainingDistance < 1.0f) {

            ResetAgent();
            int i = Random.Range(0, goalLocations.Length);
            agent.SetDestination(goalLocations[i].transform.position);
        }
    }
}