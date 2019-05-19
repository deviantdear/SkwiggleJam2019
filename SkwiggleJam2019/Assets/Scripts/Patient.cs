using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Wellness {Healthy, Unwell, Sick, CodeRed, Death}
public enum Illness {None, Infection, HeartAttack, Stroke}

public class Patient : MonoBehaviour
{
   // the system that makes a call to nurses
    Call call;

// is the patient making a call?
    public bool isCalling;
    public float health;


    [Header("SET THE VARIABLES BELOW")]
    public float sickeningRate;
    public float recoveryRate;

    public float maxHealth;
	public Illness illness;
	public bool presentNurse;
	private float baseRecovery, baseSickening;

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
    float[] wellnessPriorities;

    private void Start()
    {
		illness = Illness.None;
        health = maxHealth = 100f;
        call = GetComponent<Call>();

        wellnessPriorities = new float[4] {1,2,3,5};

		presentNurse = false;

		baseRecovery = baseSickening = Random.Range(0, 3);

		StartCoroutine(Dying());
    }

    private void Update()
    {
        // deplete health
        health -= Time.deltaTime * sickeningRate;

		if (illness == Illness.Infection) {
			recoveryRate = 2f;
			sickeningRate = 6f;

		} else if (illness == Illness.HeartAttack) {
			recoveryRate = 4f;
			sickeningRate = 4f;

		} else if (illness == Illness.Stroke) {
			recoveryRate = 6f;
			sickeningRate = 2f;

		} else {
			recoveryRate = baseRecovery;
			sickeningRate = baseSickening;
		}

        // if the patient is not making a call && is not dead
        if(!isCalling && condition != Wellness.Death)
        {
            if (health / maxHealth < 0.75f)
                // assign a call with a priority value based on condition
                call.StartCall(wellnessPriorities[Mathf.Clamp((int)condition,0,wellnessPriorities.Length-1)]);
        }
    }

	IEnumerator Dying() {
		yield return new WaitForSeconds (1.0f);

		if (presentNurse && health < maxHealth) {

			if (health > maxHealth) {
				health = maxHealth;
			}
			if (health == maxHealth) {
				illness = Illness.None;
			}

		}
	}
}