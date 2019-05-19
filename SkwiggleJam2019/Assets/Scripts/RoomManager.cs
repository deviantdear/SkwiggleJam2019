using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public List<Patient> patients;
    public List<Nurse> nurses;
    public List<Transform> desks;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AssignRooms();
    }

    void AssignRooms()
    {
        // 8 rooms minimum
        List<Patient> tempPatients = new List<Patient>(patients);
        int amtOfNurses = nurses.Count;
        print("amount of nurses " + amtOfNurses);

        var split = tempPatients.Count / amtOfNurses;
        //print(split);

        for (int i = 0; i < amtOfNurses; i++)
        {
            string debug = "nurse " + i + ": ";
            for (int j = 0; j < split; j++)
            {
                int rand = Random.Range(0, tempPatients.Count);
                debug += tempPatients[rand];
                nurses[i].assignedPatients.Add(tempPatients[rand]);
                tempPatients.RemoveAt(rand);
            }
            print(debug);
        }

        var remaining = tempPatients.Count;
        for (int i = 0; i < remaining; i++)
        {
            nurses[Random.Range(0, amtOfNurses)].assignedPatients.Add(tempPatients[i]);

            print(tempPatients[i] + " to nurse " + Random.Range(0, amtOfNurses));
        }


        
    }
    public Vector3 ClosestDesk(Vector3 pos)
    {
        float shortestDist = 100f;
        Vector3 shortestPoint = pos;
        foreach (var item in desks)
        {
            var distBetween = Vector3.Distance(item.position, pos);
            if (distBetween < shortestDist)
            {
                shortestDist = distBetween;
                shortestPoint = item.position;
            }
        }

        return shortestPoint;
    }
}
