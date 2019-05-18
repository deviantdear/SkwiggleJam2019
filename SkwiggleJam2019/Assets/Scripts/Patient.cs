using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Illness { None, Infection, HeartAttack, Stroke, Yes}
public enum Wellness { Healthy, Unwell, Sick, CodeRed, Death}
public class Patient : MonoBehaviour
{
    Call call;

    public Illness illness;

    public bool hasIllness { get { return illness != Illness.None ? true : false; } }

    public float sickeningRate;
    public float recoveryRate;

    public bool isCalling;

    public float maxHealth;
    public float health;

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


    public float[] wellnessPriorities;



    private void Start()
    {
        call = GetComponent<Call>();
    }

    private void Update()
    {
        // if the patient is not making a call
        if(!isCalling && condition != Wellness.Death)
        {
            if (health / maxHealth < 0.8f)
                call.StartCall(wellnessPriorities[Mathf.Clamp((int)condition,0,wellnessPriorities.Length-1)]);
        }

        health -= Time.deltaTime / 3f;
    }
}
