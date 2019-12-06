using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Toggler : BaseEnemy
{
	[SerializeField]
	int toggleInterval;
	[SerializeField]
	int speed;

    [SerializeField]
    float propellerSpeed;
    [SerializeField]
    float bodyTurnRate;

	PlayerController target;
	Coroutine coroutine;
	bool moving = false;

    Transform rotto;
    Transform propellerPivot;
    Transform body;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "Propeller Pivot":
                    propellerPivot = child;
                    break;
                case "Body":
                    body = child;
                    break;
                default:
                    break;
            }
        }

        rotto = GameObject.Find("Rotto").GetComponent<Transform>();
    }

    void Update()
    {
		if (target != null && !Paused)
		{
            Vector3 directionVector = body.position - rotto.position;
            directionVector.y = 0;
            float angleBetween = Vector3.SignedAngle(Vector3.back, directionVector, Vector3.up);
            body.rotation = Quaternion.Slerp(body.rotation, Quaternion.Euler(0, angleBetween, 0), bodyTurnRate);

            if (moving)
            {
                Vector3 dir = (target.transform.position - transform.position).normalized;
			    transform.Translate(new Vector3(dir.x, 0, dir.z) * speed * Time.deltaTime);
            }
			
		}

        RotatePropeller();
    }

    void RotatePropeller()
    {
        propellerPivot.rotation = Quaternion.Euler(propellerPivot.rotation.eulerAngles + Vector3.up * propellerSpeed);
    }

	public override void SetTarget(PlayerController target)
	{
		this.target = target;

		if (target != null)
			coroutine = StartCoroutine(ToggleRoutine());
		
		if (target == null)
		{
            if (coroutine != null)
			    StopCoroutine(coroutine);
			moving = false;
		}
	}

	public override void Hit(int damage)
	{
		Room.RemoveEnemy(this);
		Destroy(gameObject);
	}

	IEnumerator ToggleRoutine()
	{
		while (true)
		{
			moving = !moving;
			yield return new WaitForSeconds(toggleInterval);
		}
	}
}
