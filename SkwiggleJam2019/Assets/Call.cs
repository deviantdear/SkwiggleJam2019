using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Call : MonoBehaviour
{
    public SphereCollider col;

    
    [Header("Exposed Variables")]
    public float seconds;
    public float ringLength;
    public int amountOfRings;
    public float additionalMult;

    [HideInInspector]
    public float mult;
    // Start is called before the first frame update
    void Start()
    {
        mult = 1f;
        StartCoroutine(Expand());
    }

    IEnumerator Expand()
    {
        int currentRing = 1;
        float testTimer = 0f;
        col.enabled = true;
        col.radius = 0f;

        while(currentRing <= amountOfRings)
        {
            col.radius += Time.deltaTime/seconds * ringLength;

            if (col.radius >= ((float)currentRing / amountOfRings)*ringLength)
            {
                currentRing++;
                mult += additionalMult;
                print("Entering ring " + currentRing + " at radius " + col.radius);
            }
            testTimer += Time.deltaTime;
            yield return null;
        }

        print("ending: " + testTimer + " seconds, " + col.radius + " length");
        col.enabled = false;
        yield return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        print("yeet " + mult);
    }
}
