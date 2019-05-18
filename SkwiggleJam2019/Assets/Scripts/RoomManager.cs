using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public List<Transform> rooms;

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
        List<Transform> tempRooms = new List<Transform>(rooms);
        int amtOfNurses = 2;

        var split = tempRooms.Count / amtOfNurses;
        //print(split);

        for (int i = 0; i < amtOfNurses; i++)
        {
            string debug = "nurse " + i + ": ";
            for (int j = 0; j < split; j++)
            {
                int rand = Random.Range(0, tempRooms.Count);
                debug += tempRooms[rand];
                tempRooms.RemoveAt(rand);
            }
            //print(debug);
        }

        var remaining = tempRooms.Count;
        for (int i = 0; i < remaining; i++)
        {
            //print(tempRooms[i] + " to nurse " + Random.Range(0, amtOfNurses));
        }
    }
}
