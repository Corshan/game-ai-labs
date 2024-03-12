using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA {                          // 1) Helper class attached to Brain class.

    List<int> genes = new List<int>();      // 2) Gene list - before were hard coded, list is now flexible.
    int dnaLength = 0;
    int maxValues = 0;                      // 3) Setup gene when initialised. Set of genes will be assigned an integer value.

    public DNA(int l, int v)                // 4) DNA constructor. How long gene length and the maximum value of the gene strand.
    {
        dnaLength = l;
        maxValues = v;
        SetRandom();                        // 5a) Clears out gene strand
    }

    public void SetRandom()                 // 5b) SetRandom implementation.
    {
        genes.Clear();
        for(int i = 0; i < dnaLength; i++)
        {
            genes.Add(Random.Range(0, maxValues));  // 6) Set values between 0 and maxValues. (Floats are possible here also.)
        }
    }

    public void SetInt(int pos, int value)  // 7 Set value for position in gene sequence.
    {                                       //   Useful for hard coding values for genes if necessary.
        genes[pos] = value;
    }

    public void Combine(DNA d1, DNA d2)     // 8) Splits parents' DNA and combines them
    {
        for(int i = 0; i < dnaLength; i++)
        {
            if(i < dnaLength/2.0)           // 9) First half of gene strand is used from Parent 1 goes into gene sequence of offspring.
            {
                int c = d1.genes[i];
                genes[i] = c;
            }
            else                            // 10) Second half goes into the same offspring.
            {                               //     Previously we randomly mixed DNA for offspring.
                int c = d2.genes[i]; 
                genes[i] = c;
            }
        }
    }

    public void Mutate()                    // 11) Set random value at a particular random gene position in that sequence.
    {
        genes[Random.Range(0,dnaLength)] = Random.Range(0, maxValues);
    }

    public int GetGene(int pos)             // 12) Return the position of a given gene in a sequence.
    {
        return genes[pos];
    }

}
