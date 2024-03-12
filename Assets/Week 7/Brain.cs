using System;                                       // 1) Brain is the controller of the character
using System.Collections;                           //    and sits between the character and the DNA.
using System.Collections.Generic;                   //    It reads the DNA, determines what to do
using UnityEngine;                                  //    and tells the character what action to carry out.
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]   // 2) Required component attached to Ethan.
public class Brain : MonoBehaviour
{
    public int DNALength = 1;                       // 3) Length set to 1 just to illustrate what can be done with a single gene.
    public float timeAlive;                         // 4) Time character will live for.
    public float distanceMoved;
    public Vector3 startPos;
    public DNA dna;                                 // 5) Our DNA strand.

    private ThirdPersonCharacter m_Character;       // 6) Character script attached to prefab.
    private Vector3 m_Move;                         // 7) A couple of behaviours (animations) - move and jump.
    private bool m_Jump; 
    bool alive = true;                              // 8) Alive (or dead) - we measure timeAlive for fitness test.               

    void OnCollisionEnter(Collision obj)            // 9)  When Ethan collides with the ground, he dies.
    {                                               // 10) Ground object needs to be tagged as "dead".
        if(obj.gameObject.tag == "dead")
        {
            alive = false;
        }
    }
    
    public void Init()                              // 11) Run Init() when instance of Ethan is created.
    {
        //initialise DNALength                      // 12) DNA is only one int long. We can store 6 different values in that.
        //0 forward                                 //     We can have commands in a gene, not just binary values.
        //1 back
        //2 left
        //3 right
        //4 jump
        //5 crouch

        dna = new DNA(DNALength,6);                 // 13) Length of 1 but a mx value of 6 (0-5).

        m_Character = GetComponent<ThirdPersonCharacter>();
        timeAlive = 0;
        alive = true;
        distanceMoved = 0;
        startPos = transform.position;
    }


    void Update()
    {
       
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()                      // 14) Fixed update replaces the character controller that comes with the Ethan third person character.
    {                                               // 15) Brain code will give gene instructions and map to key press codes.
        // read DNA
        float h = 0;
        float v = 0;
        bool crouch = false;
        if(dna.GetGene(0) == 0) v = 1;              // 16) Forward
        else if(dna.GetGene(0) == 1) v = -1;        // 17) Back etc...
        else if(dna.GetGene(0) == 2) h = -1;
        else if(dna.GetGene(0) == 3) h = 1;
        else if(dna.GetGene(0) == 4) m_Jump = true;
        else if(dna.GetGene(0) == 5) crouch = true;

        m_Move = v*Vector3.forward + h*Vector3.right;
        m_Character.Move(m_Move, crouch, m_Jump);   // 16) Call with movement insturction to Ethan.
        m_Jump = false;                             // 17) After character moves, m_Jump set to false or strange things happen!

        if(alive){                                   // 18) If character is still alive then update time alive.
            timeAlive += Time.deltaTime;            // 19) Here we want to measure the time alive until they are dead (rather than clicking on them).
            distanceMoved = Vector3.Distance(startPos, transform.position);}
    }
}
