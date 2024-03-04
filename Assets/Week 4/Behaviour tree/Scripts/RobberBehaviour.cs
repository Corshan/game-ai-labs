using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour { // Going on to agent and will run

    public GameObject diamond;                 // Objects acted upon by the behaviour tree
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;

    BehaviourTree tree;
    NavMeshAgent agent;

    public enum ActionState {                   // Tracking agent state.
                                                // Things can happen on the way to a destination.
        IDLE,
        WORKING
    };

    ActionState state = ActionState.IDLE;
    Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int money = 800;

    void Start() {

        // Speed up game play.
        Time.timeScale = 5.0f;
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();

        Sequence steal = new Sequence("Steal Something");               // Set up Steal sequence
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf hasGotMoney = new Leaf("Has Money", HasMoney);
        Leaf goToBackdoor = new Leaf("Go To Backdoor", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go To Frontdoor", GoToFrontdoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        Selector opendoor = new Selector("Open Door");                  // Set up open door selector sequence

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);

        opendoor.AddChild(goToFrontdoor);                           
        opendoor.AddChild(goToBackdoor);

        steal.AddChild(invertMoney);                                    // Execute Steal sequence backwards   
        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToBackdoor);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();                                               // Call PrintTree() in 

    }

    public Node.Status HasMoney() {

        if (money < 500) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDiamond() {

        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS) {                             // If moving OR idling

            diamond.transform.parent = this.gameObject.transform;
        }
        return s;
    }

    public Node.Status GoToVan() {

        Node.Status s = GoToLocation(van.transform.position);
        if (s == Node.Status.SUCCESS) {

            money += 300;                                           // Diamond is e300.
            diamond.SetActive(false);                               // Get rid of the diamond.
        }
        return s;
    }

    public Node.Status GoToBackdoor() {

        return GoToDoor(backdoor);
    }

    public Node.Status GoToFrontdoor() {

        return GoToDoor(frontdoor);
    }

    public Node.Status GoToDoor(GameObject door) {

        Node.Status s = GoToLocation(door.transform.position);
        if (s == Node.Status.SUCCESS) {

            if (!door.GetComponent<Lock>().isLocked) {

                door.SetActive(false);
                return Node.Status.SUCCESS;
            }

            return Node.Status.FAILURE;
        }
        return s;
    }

    Node.Status GoToLocation(Vector3 destination) {

        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE) {

            agent.SetDestination(destination);
            state = ActionState.WORKING;
        } else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2.0f) {

            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        } else if (distanceToTarget < 2.0f) {

            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }

    void Update() {

        if (treeStatus != Node.Status.SUCCESS) // Running tree in update
            treeStatus = tree.Process();
    }
}
