using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFixer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustWalls()
    {
        Transform floor = GameObject.FindGameObjectWithTag("Floor").transform;

        List<Transform> walls = new List<Transform>();
        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
        {
            walls.Add(wall.transform);
        }

        foreach (Transform wall in walls)
        {
            //Since plane position is always (0,0,0) if only parent GameObject
            //is moved, 'target - initial' point is simply 'target'
            Vector3 wallDirection = wall.localPosition;
            Vector3 floorDimensions = floor.localScale;

            print("---");
            print(wall.name);
            //Wall is either to the north or to the south
            if (Mathf.Abs(wallDirection.z) > Mathf.Abs(wallDirection.x))
            {
                //North
                if (wallDirection.z > 0)
                {
                    print("North");
                    wall.localPosition = new Vector3(wall.localPosition.x, wall.localScale.y / 2, floor.lossyScale.z * 5f - wall.localScale.z / 2);
                }
                //South
                else
                {
                    print("South");
                    wall.localPosition = new Vector3(wall.localPosition.x * 5f, wall.localScale.y / 2, -floor.lossyScale.z * 5f + wall.localScale.z / 2);
                }
            }
            else
            {
                //West
                if (wallDirection.x < 0)
                {
                    print("West");
                    wall.localPosition = new Vector3(-floor.lossyScale.x * 5f + wall.localScale.x / 2, wall.localScale.y / 2, wall.localPosition.z);
                }
                //East
                else
                {
                    print("East");
                    wall.localPosition = new Vector3(floor.lossyScale.x * 5f - wall.localScale.x / 2, wall.localScale.y / 2, wall.localPosition.z);
                }
            }
        }
    }
}
