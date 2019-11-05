using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	public bool PlayerInRoom { get; private set; } = false;

	public bool IsRoomCleared { get; private set; } = false;

	public event Action RoomCleared;

	List<BaseEnemy> enemies = new List<BaseEnemy>();

    void Start()
    {
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			BaseEnemy enemy = child.GetComponent<BaseEnemy>();

			if (enemy != null)
			{
				enemies.Add(enemy);
				enemy.SetRoom(this);
			}
		}
    }

    void Update()
    {
        
    }

	public void TogglePlayer(PlayerController player)
	{
		PlayerInRoom = !PlayerInRoom;

		if (PlayerInRoom)
			ActivateEnemies(player);
	}

	public void RemoveEnemy(BaseEnemy baseEnemy)
	{
		enemies.Remove(baseEnemy);

		IsRoomCleared = enemies.Count == 0;
		if (enemies.Count == 0)
			RoomCleared();
	}

	void ActivateEnemies(PlayerController target)
	{
		foreach (BaseEnemy enemy in enemies)
			enemy.SetTarget(target);

		IsRoomCleared = enemies.Count == 0;
		if (enemies.Count == 0)
			RoomCleared();
	}
}
