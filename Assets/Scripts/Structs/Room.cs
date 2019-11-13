using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Room
{
    public Transform room;
    public Transform[] floors;
    public Transform[] walls;
    public Transform[] doors;

    public Room(Transform room)
    {
        List<Transform> floors = new List<Transform>();
        List<Transform> walls = new List<Transform>();
        List<Transform> doors = new List<Transform>();
        foreach (Transform child in room)
        {
            if (child.tag.Equals("Floor"))
                floors.Add(child);
            if (child.tag.Equals("Wall"))
                floors.Add(child);
            if (child.tag.Equals("Door"))
                floors.Add(child);
        }
        this.room = room;
        this.floors = floors.ToArray();
        this.walls = walls.ToArray();
        this.doors = doors.ToArray();
    }
}
