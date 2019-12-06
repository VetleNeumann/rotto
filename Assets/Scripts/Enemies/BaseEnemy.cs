using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
	public int Health { get; protected set; }

	protected RoomManager Room { get; private set; }

	protected bool Paused { get; }

    public virtual void SetTarget(PlayerController target)
	{

	}

	public virtual void SetRoom(RoomManager room)
	{
		Room = room;
	}

	public virtual void Hit(int damage)
	{

	}

	public void SetPauseState(bool state)
	{

	}
}
