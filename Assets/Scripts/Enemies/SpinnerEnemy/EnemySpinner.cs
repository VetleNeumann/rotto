using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpinner : BaseEnemy
{
	[SerializeField]
	int speed;

	[SerializeField]
	int spinSpeed;

	PlayerController target;
	Transform bladeGroup;

	void Start()
    {
		bladeGroup = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(new Vector3(0f, 90f, 0f), 0.1f, Space.Self);
		if (target != null)
		{
			Vector3 dir = (target.transform.position - transform.position).normalized;
			transform.Translate(new Vector3(dir.x, 0, dir.z) * speed * Time.deltaTime);

			bladeGroup.Rotate(Vector3.up, spinSpeed, Space.Self);
		}
	}

	public override void SetTarget(PlayerController target)
	{
		this.target = target;
	}

	public override void Hit(int damage)
	{
		Room.RemoveEnemy(this);
		Destroy(gameObject);
	}
}
