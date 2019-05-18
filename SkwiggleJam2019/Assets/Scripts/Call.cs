using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Call : MonoBehaviour
{
    public Patient patient;

    public SphereCollider col;

    
    [Header("Exposed Variables")]
    public float seconds;
    public float ringLength;
    public int amountOfRings;
    public float additionalMult;
    public float priority;


    [HideInInspector]
    public float mult;



    public void StartCall(float wellness)
    {
        StartCoroutine(Expand(wellness));
    }



    IEnumerator Expand(float wellness)
    {



        patient.isCalling = true;

        int currentRing = 1;
        float testTimer = 0f;
        col.enabled = true;
        col.radius = 0f;
        mult = wellness;

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
        patient.isCalling = false;
        yield return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Nurse"))
        {
            var nurse = other.GetComponent<Nurse>();
            if (nurse.state == State.Chart)
            {
                nurse.pointOfInterest = PointOfInterest.Patient;
                nurse.focusedPatient = patient;
                nurse.agent.SetDestination(transform.position);
                nurse.priorityAwareness = mult;
            }

            if(nurse.priorityAwareness < mult && nurse.state == State.Move)
            {
                nurse.pointOfInterest = PointOfInterest.Patient;
                nurse.focusedPatient = patient;
                nurse.agent.SetDestination(transform.position);
                nurse.priorityAwareness = mult;
            }
            
            
        }
    }
}
