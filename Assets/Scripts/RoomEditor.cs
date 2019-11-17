using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomFixer))]
public class RoomEditor : Editor
{
    //I couldn't find a way to have a text field for these,
    //and spending more time on it would be wasting time sorry
    float wallHeight = 3.5f;
    float wallThickness = 0.75f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Add button for spawning wall and door

        if (GUILayout.Button("Adjust All Rooms' Walls Scale"))
            AdjustWallScale();
        if (GUILayout.Button("Set Well Length Floor Length"))
            WallLengthFloorLength();

        EditorGUILayout.Space();

        RoomFixer inspector = (RoomFixer)target;
        if (GUILayout.Button("Adjust All Rooms' Walls Positions"))
            AdjustWallPos(true);
        if (GUILayout.Button("Adjust Current Wall Position"))
            AdjustWallPos(false);
        
        EditorGUILayout.Space();

        if (GUILayout.Button("NorthWest"))
            AdjustWallPos(false, Compass.NorthWest);
        if (GUILayout.Button("NorthEast"))
            AdjustWallPos(false, Compass.NorthEast);
        if (GUILayout.Button("SouthEast"))
            AdjustWallPos(false, Compass.SouthEast);
        if (GUILayout.Button("SouthWest"))
            AdjustWallPos(false, Compass.SouthWest);
    }

    void AdjustWallScale()
    {
        Transform selected = Selection.transforms[0];
        foreach (Transform child in selected.parent.transform)
        {
            if (child.tag.Equals("Wall"))
            {
                //North/South wall, Z is thickness
                if (child.localScale.x > child.localScale.z)
                {
                    child.localScale = new Vector3(child.localScale.x, wallHeight, wallThickness);
                }
                //West/East wall, X is thickness
                if (child.localScale.x < child.localScale.z)
                {
                    child.localScale = new Vector3(wallThickness, wallHeight, child.localScale.z);
                }
            }
        }
    }

    void WallLengthFloorLength()
    {
        Transform selected = Selection.transforms[0];

        Transform floor = selected;
        foreach (Transform child in selected.parent.transform)
        {
            if (child.tag.Equals("Floor"))
            {
                floor = child;
                break;
            }
        }

        //if floor is still wall, not floor (aka no floor sibling)
        if (floor == selected)
        {
            Debug.LogError("No floor found.");
            return;
        }

        if (selected.localScale.x > selected.localScale.z)
            selected.localScale = new Vector3(floor.localScale.x * 5f * 2f, selected.localScale.y, selected.localScale.z);
        if (selected.localScale.x < selected.localScale.z)
            selected.localScale = new Vector3(selected.localScale.x, selected.localScale.y, floor.localScale.z * 5f * 2f);
        AdjustWallPos(false);
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
