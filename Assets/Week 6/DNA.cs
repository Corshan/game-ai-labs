using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour {

    //gene for colour                   // 1) This is the only gene elements that will be carried to the next generation
    public float r;                     //    Could be an array, sturct etc.
    public float g;                     //    Although this particular genetic element has three elements,
    public float b;                     //    for the purpose of genetic coding, it is considered a single gene.

    bool dead = false;                  // 2) When clicked on then the person has died.
                                        //    This will set to true when clicked.
                                        //    The ones that live the longest will be the 'fittest' and will
                                        //    breed with the next generation.

    public float timeToDie = 0;         // 3) Records how long the person lived for.
                                        //    These times can then be sorted and the ones that lived the longest will
                                        //    be used for breeding later.
    SpriteRenderer sRenderer;
    Collider2D sCollider;   

    void OnMouseDown()                          // 4) No raycasts, just a mouse click.
    {
        dead = true;                            // 5) Clicked, person dies.            
        timeToDie = PopulationManager.elapsed;  // 6) And Time to Die is going to be set to the time elapsed
                                                //    in the population manager.
        //Debug.Log("Dead At: " + timeToDie);
        sRenderer.enabled = false;              // 7) Disappear and remove collider when clicked, ie dead.
        sCollider.enabled = false;              //    We don't destroy them as we may need them to breed them later.
    }   

    // Use this for initialization
    void Start () {
        sRenderer = GetComponent<SpriteRenderer>();
        sCollider = GetComponent<Collider2D>(); 
        sRenderer.color = new Color(r,b,g);         // 8) Get random colours from Population Manager.
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
