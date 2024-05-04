using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Week10
{
    public class AI : MonoBehaviour
    {           // Code attached to NPV

        NavMeshAgent agent;                     // Code to send to states
        Animator anim;
        State currentState;

        public Transform player;

        void Start()
        {

            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            currentState = new Idle(gameObject, agent, anim, player);
        }


        void Update()
        {

            currentState = currentState.Process();
        }
    }
}
