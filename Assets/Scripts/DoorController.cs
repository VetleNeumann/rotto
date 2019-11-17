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
    PlayerController playerController;

    bool open = false;
    bool moving = false;
    bool cleared = true;
    [SerializeField]
	bool locked = false;
    [SerializeField]
	bool solved = true;

    // Start is called before the first frame update
    void Awake()
    {
        doorManager = GetComponent<DoorManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        if (solved == false)
            doorManager.SetStatus(DoorStatus.Puzzle);
        else if (locked == true)
            doorManager.SetStatus(DoorStatus.Locked);
        else
            doorManager.SetStatus(DoorStatus.Open);
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

        if (locked)
            doorManager.SetStatus(DoorStatus.Locked);
        else
            doorManager.SetStatus(DoorStatus.Open);
	}

	public void EnemyLock()
	{
        /*
		if (open == true)
		{
			open = false;
			moving = true;
		}
        */

        cleared = false;
        //doorManager.ToggleLights(false);
        doorManager.SetStatus(DoorStatus.Enemies);

        StartCoroutine(ToggleDoorAfterDelay(false, 0.25f));
        StartCoroutine(ToggleLightAfterDelay(false, 0.25f));
        //locked = true;
    }

	public void EnemyUnlock()
	{
        cleared = true;
        if (!solved)
            doorManager.SetStatus(DoorStatus.Puzzle);
        else if (locked)
            doorManager.SetStatus(DoorStatus.Locked);
        else
            doorManager.SetStatus(DoorStatus.Open);
        //maybe add slight delay to togglelights, IT DOES TEMPORARILY NOT SO I AM MAKING THIS HUGE COMMENT
        //doorManager.ToggleLights(true);
        StartCoroutine(ToggleLightAfterDelay(true, 0.8f));
        //locked = false;
    }

	public void TraverseDoor(PlayerController player)
	{
        //doorManager.ToggleLights(true);
        StartCoroutine(ToggleDoorAfterDelay(false, 0.15f));

        for (int i = 0; i < connectedRooms.Count; i++)
			connectedRooms[i].TogglePlayer(player);
	}

    IEnumerator ToggleDoorAfterDelay(bool open, float delay)
    {
        print(delay);

        yield return new WaitForSeconds(delay);
        if (open && !cleared) { }
        else
        {
            this.open = open;
            moving = true;
        }
        
    }

    IEnumerator ToggleLightAfterDelay(bool on, float delay)
    {
        yield return new WaitForSeconds(delay);
        doorManager.ToggleLights(on);
    }

    public override void ButtonPressed()
    {
        if (!moving && cleared)
        {
            if (!solved)
                return;
            else if (locked)
            {
                if (playerController.keys == 0)
                {
                    StartCoroutine(doorManager.BlinkLight());
                }
                else
                {
                    playerController.keys--;
                    StartCoroutine(doorManager.UnlockDoor());
                    locked = false;
                }
            }
            else
            {
                doorManager.ToggleLights(false);
                open = !open;
                moving = true;
            }
        }
	}

	public void AddConnectedRoom(RoomController room)
	{
		connectedRooms.Add(room);
	}
}
