using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Wellness { Healthy, Unwell, Sick, CodeRed, Death}


public class Patient : MonoBehaviour
{
    // the system that makes a call to nurses
    Call call;

    public float sickeningRate;
    public float recoveryRate;


    // is the patient making a call?
    public bool isCalling;

    public float maxHealth;
    public float health;

    // get the player's condition
    public Wellness condition
    {
        get
        {
            var healthPercent = health / maxHealth;
            return healthPercent > 0.75f ? Wellness.Healthy :
                healthPercent > .60f ? Wellness.Unwell :
                healthPercent > .40f ? Wellness.Sick :
                healthPercent > 0f ? Wellness.CodeRed : Wellness.Death;
        }
    }


    // increase the priorities based on the wellness
    public float[] wellnessPriorities;

    


    private void Start()
    {
        call = GetComponent<Call>();
    }

    private void Update()
    {
        // if the patient is not making a call && is not dead
        if(!isCalling && condition != Wellness.Death)
        {
            if (health / maxHealth < 0.75f)
                // assign a call with a priority value based on condition
                call.StartCall(wellnessPriorities[Mathf.Clamp((int)condition,0,wellnessPriorities.Length-1)]);
        }
    }
}
