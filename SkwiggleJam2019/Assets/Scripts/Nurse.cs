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
    public List<Patient> assignedPatients;
    [Header("SET THE VARIABLES BELOW")]
	public float healRate;
	public float baseHeal;
    public float mournTime;
    float mournTimer;


   

	public GameObject healing, crying;
	public bool fear;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

            focusedPatient.health = Mathf.Clamp(focusedPatient.health += Time.deltaTime * healRate, 0, focusedPatient.maxHealth);
			focusedPatient.presentNurse = true;

            if (focusedPatient.health == focusedPatient.maxHealth)
            {
                state = State.Move;
                agent.isStopped = false;
                pointOfInterest = PointOfInterest.Computer;
                agent.SetDestination(RoomManager.instance.ClosestDesk(transform.position));
                priorityAwareness = 0f;

				healing.SetActive(false);
				focusedPatient.presentNurse = false;
            }

        }
        else if (state == State.Mourn)
        {
			crying.SetActive(true);

            mournTimer += Time.deltaTime;

            if(mournTimer >= mournTime)
            {
                mournTimer = 0f;
                state = State.Move;
                agent.isStopped = false;
                pointOfInterest = PointOfInterest.Computer;
                agent.SetDestination(RoomManager.instance.ClosestDesk(transform.position));
                priorityAwareness = 0f;
				crying.SetActive(false);
            }

        }
    }
}