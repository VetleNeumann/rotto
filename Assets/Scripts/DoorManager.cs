using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    Transform leftDoor;
    Transform rightDoor;

    //How much of the door should remain in sight
    float doorRemain = 0.25f;
    float closedTolerance = 0.05f;
    float doorWidth = 1f;
    float doorSpeed = 0.08f;

    Vector3 initialPosLeft;
    Vector3 initialPosRight;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "Door Left":
                    leftDoor = child;
                    initialPosLeft = leftDoor.position;
                    break;
                case "Door Right":
                    rightDoor = child;
                    initialPosRight = rightDoor.position;
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool MoveDoor(bool open)
    {
        leftDoor.position = Vector3.Lerp(leftDoor.position, initialPosLeft - transform.right * (open ? 1f - doorRemain : 0f), doorSpeed);
        rightDoor.position = Vector3.Lerp(rightDoor.position, initialPosRight + transform.right * (open ? 1f - doorRemain : 0f), doorSpeed);
        
        if (open && (leftDoor.position - (initialPosLeft - transform.right * (1 - doorRemain))).magnitude < closedTolerance)
        {
            leftDoor.position = initialPosLeft - transform.right * (1 - doorRemain);
            rightDoor.position = initialPosRight + transform.right * (1 - doorRemain);
            return true;
        }
        else if (!open && (leftDoor.position - initialPosLeft).magnitude < closedTolerance)
        {
            leftDoor.position = initialPosLeft;
            rightDoor.position = initialPosRight;
            return true;
        }
        return false;
    }
}
