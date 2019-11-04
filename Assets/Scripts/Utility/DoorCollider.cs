using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
	DoorController door;

    void Start()
    {
		door = GetComponentInParent<DoorController>();
    }

	private void OnTriggerEnter(Collider other)
	{
		PlayerController playerController = other.GetComponent<PlayerController>();
		if (playerController != null)
			door.TraverseDoor(playerController);
	}
}
