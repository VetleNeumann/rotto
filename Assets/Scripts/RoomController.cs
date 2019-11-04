using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomManager))]
public class RoomController : MonoBehaviour
{
	[SerializeField]
	List<DoorController> doors = new List<DoorController>();

	RoomManager roomManager;

    void Awake()
    {
		roomManager = GetComponent<RoomManager>();

		roomManager.RoomCleared += RoomCleared;
    }

	private void Start()
	{
		for (int i = 0; i < doors.Count; i++)
			doors[i].AddConnectedRoom(this);
	}

	void Update()
    {
        
    }

	public void TogglePlayer(PlayerController player)
	{
		roomManager.TogglePlayer(player);

		if (roomManager.PlayerInRoom && !roomManager.IsRoomCleared)
		{
			for (int i = 0; i < doors.Count; i++)
				doors[i].LockDoor();
		}
	}

	void RoomCleared()
	{
		for (int i = 0; i < doors.Count; i++)
			doors[i].UnlockDoor();
	}
}
