using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Doctor : MonoBehaviour
{
    public NavMeshAgent agent;
    public PointOfInterest pointOfInterest;
    public State state;
    public Patient focusedPatient;

    public float priorityAwareness;

    public float mournTime = 5f;
    float mournTimer;

    float chartTimer;
    public float chartTime;
    int patientIndex;

    public List<Patient> assignedPatients;


    private void Start()
    {
        state = State.Chart;
        chartTimer = 0f;
    }

    private void Update()
    {


        if (state == State.Chart)
        {
            chartTimer++;
            if (chartTimer > chartTime)
            {
                agent.isStopped = false;
                state = State.Move;
                pointOfInterest = PointOfInterest.Patient;
                focusedPatient = RoomManager.instance.patients[patientIndex];
                agent.SetDestination(RoomManager.instance.patients[patientIndex].transform.position);
                patientIndex++;
                if (patientIndex > RoomManager.instance.patients.Count - 1)
                    patientIndex = 0;

            }
        }
        else if (state == State.Move)
        {

            if (agent.remainingDistance < 2f)
            {
                agent.isStopped = true;

                if (patientIndex > RoomManager.instance.patients.Count - 1)
                {
                    agent.SetDestination(RoomManager.instance.ClosestDesk(transform.position));
                }

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
                    {
                        agent.isStopped = false;
                        state = State.Move;
                        pointOfInterest = PointOfInterest.Patient;
                        focusedPatient = RoomManager.instance.patients[patientIndex];
                        agent.SetDestination(RoomManager.instance.patients[patientIndex].transform.position);
                        patientIndex++;
                        if (patientIndex > RoomManager.instance.patients.Count - 1)
                            patientIndex = 0;
                    }
                }
            }
        }
        else if (state == State.Mourn)
        {
            mournTimer += Time.deltaTime;

            if (mournTimer >= mournTime)
            {
                mournTimer = 0f;
                state = State.Move;
                agent.isStopped = false;
                pointOfInterest = PointOfInterest.Computer;
                agent.SetDestination(RoomManager.instance.ClosestDesk(transform.position));
                priorityAwareness = 0f;
            }

        }


        foreach (var item in RoomManager.instance.nurses)
        {
            if (Vector3.Distance(transform.position, item.transform.position) < 3f)
            {
                item.healRate *= 1.5f;
            }
        
        }

    }

}
