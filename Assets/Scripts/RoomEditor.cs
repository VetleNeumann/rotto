using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomFixer))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomFixer inspector = (RoomFixer)target;
        if (GUILayout.Button("Adjust All Rooms' Walls"))
            AdjustWallPos(true);
        if (GUILayout.Button("Adjust Current Wall"))
            AdjustWallPos(false);

        if (GUILayout.Button("NorthEast"))
            AdjustWallPos(false, Compass.NorthEast);
        if (GUILayout.Button("SouthEast"))
            AdjustWallPos(false, Compass.SouthEast);
        if (GUILayout.Button("SouthWest"))
            AdjustWallPos(false, Compass.SouthWest);
        if (GUILayout.Button("NorthWest"))
            AdjustWallPos(false, Compass.NorthWest);
    }

    void AdjustWallPos(bool allWalls)
    {
        //Transform selected = (Transform)target;
        Transform selected = Selection.transforms[0];

        Transform floor = selected;
        List<Transform> walls = new List<Transform>();
        foreach (Transform child in selected.parent.transform)
        {
            switch (child.tag)
            {
                case "Floor":
                    floor = child;
                    break;
                case "Wall":
                    walls.Add(child);
                    break;
                default:
                    break;
            }
        }

        //if floor is still wall, not floor (aka no floor sibling)
        if (floor == selected)
        {
            Debug.LogError("No floor found.");
            return;
        }
            

        foreach (Transform wall in walls)
        {
            if (!allWalls && wall != selected)
                continue;

            //Since plane position is always (0,0,0) if only parent GameObject
            //is moved, 'target - initial' point is simply 'target'
            Vector3 wallDirection = wall.localPosition;
            Vector3 floorDimensions = floor.localScale;

            //Wall is either to the north or to the south
            if (Mathf.Abs(wallDirection.z) > Mathf.Abs(wallDirection.x))
            {
                //North
                if (wallDirection.z > 0)
                {
                    wall.localPosition = new Vector3(wall.localPosition.x, wall.localScale.y / 2, floor.lossyScale.z * 5f - wall.localScale.z / 2);
                }
                //South
                else
                {
                    wall.localPosition = new Vector3(wall.localPosition.x, wall.localScale.y / 2, -floor.lossyScale.z * 5f + wall.localScale.z / 2);
                }
            }
            else
            {
                //West
                if (wallDirection.x < 0)
                {
                    wall.localPosition = new Vector3(-floor.lossyScale.x * 5f + wall.localScale.x / 2, wall.localScale.y / 2, wall.localPosition.z);
                }
                //East
                else
                {
                    wall.localPosition = new Vector3(floor.lossyScale.x * 5f - wall.localScale.x / 2, wall.localScale.y / 2, wall.localPosition.z);
                }
            }
        }
    }

    void AdjustWallPos(bool allWalls, Compass alignDir)
    {
        //Transform selected = (Transform)target;
        Transform wall = Selection.transforms[0];

        Transform floor = wall;
        foreach (Transform child in wall.parent.transform)
        {
            if (child.tag.Equals("Floor"))
            {
                floor = child;
                break;
            }
        }

        //if floor is still wall, not floor (aka no floor sibling)
        if (floor == wall)
        {
            Debug.LogError("No floor found.");
            return;
        }

        //Since plane position is always (0,0,0) if only parent GameObject
        //is moved, 'target - initial' point is simply 'target'
        Vector3 wallDirection = wall.localPosition;
        Vector3 floorDimensions = floor.localScale;

        //Wall is either to the north or to the south
        switch (alignDir)
        {
            case Compass.NorthEast:
                wall.localPosition = new Vector3(floor.lossyScale.x * 5f - wall.lossyScale.x / 2, wall.lossyScale.y / 2, floor.lossyScale.z * 5f - wall.lossyScale.z / 2);
                break;
            case Compass.SouthEast:
                wall.localPosition = new Vector3(floor.lossyScale.x * 5f - wall.lossyScale.x / 2, wall.lossyScale.y / 2, -floor.lossyScale.z * 5f + wall.lossyScale.z / 2);
                break;
            case Compass.SouthWest:
                wall.localPosition = new Vector3(-floor.lossyScale.x * 5f + wall.lossyScale.x / 2, wall.lossyScale.y / 2, -floor.lossyScale.z * 5f + wall.lossyScale.z / 2);
                break;
            case Compass.NorthWest:
                wall.localPosition = new Vector3(-floor.lossyScale.x * 5f + wall.lossyScale.x / 2, wall.lossyScale.y / 2, floor.lossyScale.z * 5f - wall.lossyScale.z / 2);
                break;
            default:
                break;
        }
    }
}
