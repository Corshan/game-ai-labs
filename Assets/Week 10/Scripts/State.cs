using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/* A state machine for an NPC guard who will patrol an area and pursue an attack approaching players. 
   State based class that all other states will inherit from.
*/


public class State {

    public enum STATE {

        IDLE,             // A state records the type of state it is,
        PATROL,           // which will entirely depend on the type of state machine you
        PURSUE,           // are writing.
        ATTACK,
        SLEEP
    };

    public enum EVENT {

        ENTER,            // Three phases of the state.
        UPDATE,           // The phase will dictate which method to run.
        EXIT
    };
                                // Item values specific to the game.
    public STATE name;          // One of Idle, Patrol etc.
    protected EVENT stage;      // One of Enter, Update or Exit
    protected GameObject npc;
    protected Animator anim;
    protected Transform player; // Guard needs to know where the player is.
    protected State nextState;  // Next state
    protected NavMeshAgent agent;

    float visDist = 10.0f;      // Visual distance and angle, and shoot distance to determine
    float visAngle = 30.0f;     // sight and attack state threshold.
    float shootDist = 7.0f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) { // Constructor for State()

        npc = _npc;
        agent = _agent;
        anim = _anim;
        player = _player;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }   // You can see that there are methods for each of the stages
    public virtual void Update() { stage = EVENT.UPDATE; }  // and the base methods update the stages accordingly.
    public virtual void Exit() { stage = EVENT.EXIT; }      // Current state and next stage

    public State Process() {

    /* The process method is called from outside of any state and it determines which method inside the state should run
       at any time based on the set stage. Through the use of this base class as a template for all states,
       we can be sure to keep the code related to each state compartmentalized from other NPC behaviors.
    */
        if (stage == EVENT.ENTER) Enter();      // Current state and next stage again.
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT) {

            Exit();
            return nextState;
        }

        return this;                             // If not returning the next state then return this.
    }

    public bool CanSeePlayer() {                // Line of sight calculation (distance and angle thresholds)

        Vector3 direction = player.position - npc.transform.position;  // Vector from NPC to Player
        float angle = Vector3.Angle(direction, npc.transform.forward); // Angle of sight

        if (direction.magnitude < visDist && angle < visAngle) {       // visAngle 1/2 of angle of sight   

            return true;
        }

        return false;
    }

    public bool CanAttackPlayer() {                                     // Attack threshold

        Vector3 direction = player.position - npc.transform.position;
        if (direction.magnitude < shootDist) {                          // Check distance only of NPC to player.

            return true;
        }

        return false;
    }
}

public class Idle : State {

    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) // Constructor
        : base(_npc, _agent, _anim, _player) {

        name = STATE.IDLE;
    }
                                        // Overrides for Enter, Update and Exit
    public override void Enter() {

        anim.SetTrigger("isIdle");      // Trigger animation
        base.Enter();                   // This sets the base to update()
    }

    public override void Update() {     // Update for Idle - transition to next state

        if (CanSeePlayer()) {

            nextState = new Pursue(npc, agent, anim, player); // Exit this update 
            stage = EVENT.EXIT;
        } else if (Random.Range(0, 100) < 10) {               // 10% of the time, go to next state

            nextState = new Patrol(npc, agent, anim, player);   
            stage = EVENT.EXIT;                               // Run Exit state which will run next state (Patrol)                           
        }
    }

    public override void Exit() {       // Any states not used are overridden

        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}

public class Patrol : State {

    int currentIndex = -1;          // Index of waypoints

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) // Constructor
        : base(_npc, _agent, _anim, _player) {

        name = STATE.PATROL;
        agent.speed = 2.0f;       // Patrol speed
        agent.isStopped = false;  // Stops if the agent does something while on the way to a waypoint.

    }

    public override void Enter() {

        float lastDistance = Mathf.Infinity;

        for (int i = 0; i < GameEnvironment.Singleton.Checkpoints.Count; ++i) {

            GameObject thisWP = GameEnvironment.Singleton.Checkpoints[i];
            float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
            if (distance < lastDistance) {

                currentIndex = i - 1;
                lastDistance = distance;
            }
        }

        anim.SetTrigger("isWalking");
        base.Enter();
    }

    public override void Update() {

        if (agent.remainingDistance < 1) {    // While agent is still walking then move to the waypoint

            if (currentIndex >= GameEnvironment.Singleton.Checkpoints.Count - 1) {

                currentIndex = 0;             // Recycle waypoints
            } else {

                currentIndex++;               // Go the the next waypoint.
            }

            agent.SetDestination(GameEnvironment.Singleton.Checkpoints[currentIndex].transform.position); // Set destination
        }

        if (CanSeePlayer()) {                                   // If you are in pursue and player is close enough
                                                                // then NPC will attack.
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }                                                       // Note: Idle references Patrol,
    }                                                           //       Patrol does not reference another state.

    public override void Exit() {

        anim.ResetTrigger("isWalking");     // Reset walking trigger.
        base.Exit();
    }
}

public class Pursue : State {

    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player) {


        name = STATE.PURSUE;
        agent.speed = 5.0f;
        agent.isStopped = false;
    }

    public override void Enter() {

        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update() {

        agent.SetDestination(player.position);

        if (agent.hasPath) {                // If NPC is following the player.

            if (CanAttackPlayer()) {

                nextState = new Attack(npc, agent, anim, player);
                stage = EVENT.EXIT;
            } else if (!CanSeePlayer()) {

                nextState = new Patrol(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit() {

        anim.ResetTrigger("isRunning");
        base.Exit();
    }
}

public class Attack : State {

    float rotationSpeed = 2.0f;                         // Rotate component
    AudioSource shoot;

    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player) {

        name = STATE.ATTACK;
        shoot = _npc.GetComponent<AudioSource>();       // Audio component
    }

    public override void Enter() {

        anim.SetTrigger("isShooting");
        agent.isStopped = true;                         // NPC will stop to attack.
        shoot.Play();
        base.Enter();
    }

    public override void Update() {

        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);  // direction is the vector toward the player
        direction.y = 0.0f;                                             // Prevents NPC tilting on height.

        npc.transform.rotation =
            Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);

        if (!CanAttackPlayer()) {                                       // If player is out of range.

            nextState = new Idle(npc, agent, anim, player);             // Next state is Idle (which can then go to
            shoot.Stop();                                               // any other state).
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {

        anim.ResetTrigger("isShooting");
        base.Exit();
    }
}

// NOTE: The PlayerCapsule element of the FPCharacter should be copied
//       to the Player component of the NPC's AI script. 