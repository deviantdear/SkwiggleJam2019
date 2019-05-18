using System.Collections;
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
    private void Start()
    {
        state = State.Chart;
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


        if (state == State.Move)
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

        if (state == State.Heal)
        {
            focusedPatient.health = Mathf.Clamp(focusedPatient.health += Time.deltaTime * 3f, 0, focusedPatient.maxHealth);

            if (focusedPatient.health == focusedPatient.maxHealth)
            {
                state = State.Move;
                agent.isStopped = false;
                pointOfInterest = PointOfInterest.Computer;
                agent.SetDestination(Vector3.zero);
            }

        }


        if (state == State.Mourn)
        {
            mournTimer += Time.deltaTime;

            if(mournTimer >= mournTime)
            {
                state = State.Move;
                agent.isStopped = false;
                pointOfInterest = PointOfInterest.Computer;
                agent.SetDestination(Vector3.zero);
            }

        }
    }
}
