using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshRenderer))]
public class Toggler : BaseEnemy
{
	[SerializeField]
	int toggleInterval;
	[SerializeField]
	int speed;

	[SerializeField]
	Material onMaterial;
	[SerializeField]
	Material offMaterial;

	MeshRenderer renderer;

	PlayerController target;
	Coroutine coroutine;
	bool vulnerable = false;

    void Awake()
    {
		renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
		if (target != null)
		{
			Vector3 dir = (target.transform.position - transform.position).normalized;
			transform.Translate(new Vector3(dir.x, 0, dir.z) * speed * Time.deltaTime);
		}
    }

	public override void SetTarget(PlayerController target)
	{
		this.target = target;

		if (target != null)
			coroutine = StartCoroutine(ToggleRoutine());
		
		if (target == null)
		{
			StopCoroutine(coroutine);
			vulnerable = false;
			renderer.material = offMaterial;
		}
	}

	public override void Hit(int damage)
	{
		if (vulnerable)
		{
			Room.RemoveEnemy(this);
			Destroy(gameObject);
		}
	}

	IEnumerator ToggleRoutine()
	{
		while (true)
		{
			if (vulnerable)
				renderer.material = onMaterial;
			else
				renderer.material = offMaterial;

			vulnerable = !vulnerable;
			yield return new WaitForSeconds(toggleInterval);
		}
	}
}
