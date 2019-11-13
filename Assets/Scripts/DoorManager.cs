using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    Transform leftDoor;
    Transform rightDoor;

    Renderer[] buttons;
    Light[] lights;
    [SerializeField]
    Material matOpen, matLocked, matPuzzle, matEnemies;

    //How much of the door should remain in sight
    float doorRemain = 0.25f;
    float closedTolerance = 0.05f;
    float doorWidth = 1f;
    float doorSpeed = 0.08f;

    Vector3 initialPosLeft;
    Vector3 initialPosRight;

    bool blinking = false;

    // Start is called before the first frame update
    void Awake()
    {
        buttons = new Renderer[2];
        lights = new Light[2];

        int buttonIndex = 0;
        int lightIndex = 0;
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
            if (child.tag.Equals("Button"))
            {
                buttons[buttonIndex] = child.GetComponent<Renderer>();
                buttonIndex++;
            }
            else if (child.tag.Equals("Light"))
            {
                lights[lightIndex] = child.GetComponent<Light>();
                lightIndex++;
            }
        }
    }

    void Update()
    {
        
    }

    public void SetStatus(DoorStatus status)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            switch (status)
            {
                case DoorStatus.Open:
                    buttons[i].material = matOpen;
                    lights[i].color = Color.green;
                    break;
                case DoorStatus.Locked:
                    buttons[i].material = matLocked;
                    lights[i].color = Color.red;
                    break;
                case DoorStatus.Puzzle:
                    buttons[i].material = matPuzzle;
                    lights[i].color = Color.blue;
                    break;
                case DoorStatus.Enemies:
                    buttons[i].material = matEnemies;
                    lights[i].color = Color.red;
                    break;
                default:
                    break;
            }
        }        
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

    public IEnumerator BlinkLight(float duration = 0.2f, int blinks = 3)
    {
        if (!blinking)
        {
            blinking = true;
            for (int i = 0; i < blinks * 2; i++)
            {
                for (int o = 0; o < lights.Length; o++)
                {
                    lights[o].enabled = !lights[o].enabled;
                }
                yield return new WaitForSeconds(duration);
            }
            blinking = false;
        }
        yield return new WaitForSeconds(0);
    }

    public IEnumerator UnlockDoor()
    {
        ToggleLights(false);
        yield return new WaitForSeconds(0.25f);
        SetStatus(DoorStatus.Open);
        ToggleLights(true);
        //play sound

    }

    public void ToggleLights(bool on)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = on;
        }
    }
}
