using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoorManager))]
public class DoorController : MonoBehaviour
{
    DoorManager doorManager;

    bool open = false;
    bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        doorManager = GetComponent<DoorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
            moving = !doorManager.MoveDoor(open);
    }

    public void ToggleDoor()
    {
        open = !open;
        moving = true;
    }
}
