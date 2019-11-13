using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
//[CustomEditor(typeof(RoomManager))]
public class RoomEditor : Editor
{
    [SerializeField]
    float wallWidth, wallHeight;

    List<Room> rooms;

    // Start is called before the first frame update
    void Start()
    {
        rooms = new List<Room>();
        foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
        {
            rooms.Add(new Room(room.transform));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        foreach (Room room in rooms)
        {
            Vector3[] corners = GetCorners(room);
            for (int i = 0; i < corners.Length; i++)
                corners[i] += Vector3.up * (wallHeight + 1f);
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
    }

    void OnSceneGUI()
    {
        Vector3[] corners = GetCorners(rooms[0]);
        Handles.color = Handles.zAxisColor;
        Handles.ArrowHandleCap(
                0,
                corners[0] + (corners[0] - corners[1]) / 2,
                Quaternion.Euler(Vector3.forward),
                1f,
                EventType.Repaint
        );
    }

    Vector3[] GetCorners(Room room)
    {
        //Assumes floor is only 1 plane
        Vector3[] corners = new Vector3[4];
        //A plane with scale 1 will be 10x10 in size
        //Divided by 2 because distance from center to edge will be
        //half of the total width/length.
        Vector3 floorSize = room.floors[0].lossyScale * 10 / 2;
        floorSize.y = 0;
        corners[0] = room.room.position + new Vector3(-floorSize.x, 0, +floorSize.z);
        corners[1] = room.room.position + new Vector3(+floorSize.x, 0, +floorSize.z);
        corners[2] = room.room.position + new Vector3(+floorSize.x, 0, -floorSize.z);
        corners[3] = room.room.position + new Vector3(-floorSize.x, 0, -floorSize.z);

        return corners;
    }
}
