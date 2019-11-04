using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoorManager))]
public class DoorController : ControllerBase
{
	public bool Locked
	{
		get
		{
			return locked;
		}
	}

    DoorManager doorManager;
	List<RoomController> connectedRooms = new List<RoomController>();

    bool open = false;
    bool moving = false;
	bool locked = false;
	bool solved = false;

    // Start is called before the first frame update
    void Awake()
    {
        doorManager = GetComponent<DoorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
            moving = !doorManager.MoveDoor(open);
    }

	public void Solve()
	{
		solved = true;

		if (!locked)
		{
			open = true;
			moving = true;
		}
	}

	public void LockDoor()
	{
		if (open == true)
		{
			open = false;
			moving = true;
		}

		locked = true;
	}

	public void UnlockDoor()
	{
		locked = false;

		if (solved)
		{
			open = true;
			moving = true;
		}
	}

	public void TraverseDoor(PlayerController player)
	{
		for (int i = 0; i < connectedRooms.Count; i++)
			connectedRooms[i].TogglePlayer(player);
	}

    public override void ButtonPressed()
    {
		if (!locked)
		{
			open = !open;
			moving = true;
		}
	}

	public void AddConnectedRoom(RoomController room)
	{
		connectedRooms.Add(room);
	}
}
