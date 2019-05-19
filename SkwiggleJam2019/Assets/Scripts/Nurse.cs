﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State { Red, Move, Chart, Mourn, Heal}
public enum PointOfInterest { None, Patient, Computer}

public class Nurse : MonoBehaviour
{
    public NavMeshAgent agent;
    public PointOfInterest pointOfInterest;
    public State state;
    public Patient focusedPatient;

    public float priorityAwareness;

    public float mournTime = 5f;
    float mournTimer;


    public List<Patient> assignedPatients;

	public GameObject healing, crying;
	public bool fear;

    private void Start()
    {
        state = State.Chart;

		healing.SetActive(false);
		crying.SetActive(false);
		fear = false;
    }

    private void Update()
    {
        

        if(state == State.Chart)
        {
            if (pointOfInterest == PointOfInterest.Patient)
            {
                agent.isStopped = false;
                state = State.Move;
            }
        }
        else if (state == State.Move)
        {
            if(agent.remainingDistance < 2f)
            {
                agent.isStopped = true;

                if (pointOfInterest == PointOfInterest.Computer)
                {
                    state = State.Chart;
                    pointOfInterest = PointOfInterest.None;
                }
                else if (pointOfInterest == PointOfInterest.Patient)
                {
                    if (focusedPatient.condition == Wellness.Death)
                    {
                        state = State.Mourn;
                        mournTimer = 0f;
                    }
                    else
                        state = State.Heal;
                }

                pointOfInterest = PointOfInterest.None;
            }
        }
        else if (state == State.Heal)
        {
			healing.SetActive(true);

            focusedPatient.health = Mathf.Clamp(focusedPatient.health += Time.deltaTime * 3f, 0, focusedPatient.maxHealth);

            if (focusedPatient.health == focusedPatient.maxHealth)
            {
                state = State.Move;
                agent.isStopped = false;
                pointOfInterest = PointOfInterest.Computer;
                agent.SetDestination(Vector3.zero);
                priorityAwareness = 0f;
            }

        }
        else if (state == State.Mourn)
        {
			crying.SetActive(true);

            mournTimer += Time.deltaTime;

            if(mournTimer >= mournTime)
            {
                state = State.Move;
                agent.isStopped = false;
                pointOfInterest = PointOfInterest.Computer;
                agent.SetDestination(Vector3.zero);
                priorityAwareness = 0f;
            }

        }

		if (state != State.Heal) {
			healing.SetActive(false);
		} else if (state != State.Mourn) {
			crying.SetActive(false);
		}
    }
}
