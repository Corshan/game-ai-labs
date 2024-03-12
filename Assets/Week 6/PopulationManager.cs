using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;                                          // 12) Linq for sorting lists.

namespace week6
{
    public class PopulationManager : MonoBehaviour
    {

        public GameObject personPrefab;                         // 1) Person prefab
        public int populationSize = 10;                         // 2) Population size (bigger pop size, better outcomes)
        List<GameObject> population = new List<GameObject>();   // 3) List of people created.
        public static float elapsed = 0;                        // 4) Timer
        [SerializeField] float trialTime = 10;
        int generation = 1;

        GUIStyle guiStyle = new GUIStyle();                     // 9) OnGui for screen display.
        void OnGUI()
        {
            guiStyle.fontSize = 50;
            guiStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + generation, guiStyle);
            GUI.Label(new Rect(10, 65, 100, 20), "Trial Time: " + (int)elapsed, guiStyle);
        }

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < populationSize; i++)                                         // 5) Create a population
            {
                Vector3 pos = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);   // 6) Random position
                GameObject go = Instantiate(personPrefab, pos, Quaternion.identity);        // 7) Init game object, no rotations
                go.GetComponent<DNA>().r = Random.Range(0.0f, 1.0f);                         // 8) Set DNA, Random colour (R,G,B)
                go.GetComponent<DNA>().g = Random.Range(0.0f, 1.0f);                         //    111 = White, 000 = Black
                go.GetComponent<DNA>().b = Random.Range(0.0f, 1.0f);
                float randomNum = Random.Range(0.5f, 2);
                go.transform.localScale = new Vector3(randomNum, randomNum, randomNum);
                population.Add(go);
            }
        }

        GameObject Breed(GameObject parent1, GameObject parent2)                // 19) Order doesn't matter (Random DNA breeding).
        {
            Vector3 pos = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
            GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity); // 20) Create instance of the person.

            // 21) Instead of generating random RGB values
            //     we need to get (inherit) these values
            //     from the parents.
            DNA dna1 = parent1.GetComponent<DNA>();                     // 22) Getting DNA from the parent (from DNA code).                       
            DNA dna2 = parent2.GetComponent<DNA>();


            // 23) Swap parent DNA.
            if (Random.Range(0, 1000) > 5)                                // 24) The higher the range value, the more                                    
            {

                // 25) Standard code in genetic programming:
                //     Every genetic value that you're storing
                //     is randomly SWAPPED between the parents
                //     (not combined).

                offspring.GetComponent<DNA>().r = Random.Range(0, 10) < 5 ? dna1.r : dna2.r; // 26) 50% of the time the offspring
                offspring.GetComponent<DNA>().g = Random.Range(0, 10) < 5 ? dna1.g : dna2.g; //     will get its parent's R channel value.
                offspring.GetComponent<DNA>().b = Random.Range(0, 10) < 5 ? dna1.b : dna2.b; //     the other 50% of the time it will
                offspring.transform.localScale = Random.Range(0, 10) < 5 ? dna1.transform.localScale : dna2.transform.localScale; //     the other 50% of the time it will

            }                                                                               //     the other parent's R channel value.
            else
            {
                offspring.GetComponent<DNA>().r = Random.Range(0.0f, 1.0f);
                offspring.GetComponent<DNA>().g = Random.Range(0.0f, 1.0f);
                offspring.GetComponent<DNA>().b = Random.Range(0.0f, 1.0f);
                float randomNum = Random.Range(0.5f, 1.5f);
                offspring.transform.localScale = new Vector3(randomNum, randomNum, randomNum);
            }
            return offspring;
        }

        void BreedNewPopulation() // 13)
        {
            List<GameObject> newPopulation = new List<GameObject>();
            //get rid of unfit individuals
            // List<GameObject> sortedList = population.OrderByDescending(o => o.GetComponent<DNA>().timeToDie).ToList();
            List<GameObject> sortedList = population.OrderBy(_ => Random.Range(0, 11)).ToList();

            // 14) Order by timeToDie
            //     People with longer times will be at the end of the list.
            population.Clear();                                 // 15) Clear this list and breed the fit bottom half to produce
                                                                //     another population of the same size.
                                                                //breed upper half of sorted list
            for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
            {
                population.Add(Breed(sortedList[i], sortedList[i + 1]));    // 16) Breeding a person with its plus one.
                population.Add(Breed(sortedList[i + 1], sortedList[i]));    // 17) Breeding a plus one with a person.
            }


            for (int i = 0; i < sortedList.Count; i++)
            {
                Destroy(sortedList[i]);                                     // 18) destroy all parents and previous population
            }
            generation++;
        }

        // Update is called once per frame
        void Update()
        {
            elapsed += Time.deltaTime;          // 10) If elapsed time is greater than the trial time
            if (elapsed > trialTime)             //     Then if the elapsed time has become greater than
            {                                   //     the trial time, the trial is over.
                BreedNewPopulation();           // 11) This new population is a collection of the 'fittest'
                                                //     and can be bred together.
                elapsed = 0;
            }
        }
    }
}
