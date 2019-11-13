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
    List<Renderer> renderers = new List<Renderer>();

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

            Renderer renderer = transform.GetChild(i).GetComponent<Renderer>();
            if (renderer != null)
                renderers.Add(renderer);
		}

        //StartCoroutine(Fade(true));
    }

    void Update()
    {
        
    }

    public IEnumerator Fade(bool fadeOut, float fadeRate = 0.1f)
    {
        float alpha = fadeOut ? 1f : 0f;
        float goal  = fadeOut ? 0f : 1f;
        while (alpha != goal)
        {
            //Debug.Log(alpha);
            alpha = Mathf.Lerp(alpha, goal, fadeRate);
            if (Mathf.Abs(alpha - goal) < 0.05f)
                alpha = goal;
            foreach (Renderer renderer in renderers)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
            yield return new WaitForSeconds(0);
        }
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

    void SetObjectAlpha(float alpha)
    {

    }
}
