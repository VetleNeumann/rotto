using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
	Transform batonPivot;
	Transform baton;
	Transform body;

	Rigidbody2D rigidBody;

	float batonExtend;
	[SerializeField]
	float springCharge;
	bool springCharing = false;

	// Start is called before the first frame update
	void Start()
	{
		rigidBody = gameObject.GetComponent<Rigidbody2D>();

		foreach (Transform child in transform)
		{
			switch (child.name)
			{
				case "Baton Pivot":
					batonPivot = child;
					baton = batonPivot.GetComponentInChildren<Transform>();
					break;
				case "Body":
					body = child;
					break;
				default:
					break;
			}
		}
	}

	public void BatonRotate(Vector3 position, float maxMove, float inertia, bool chargeSpring)
	{
		float mouseAngle = Vector3.SignedAngle(position - transform.position, Vector3.up, Vector3.forward);
		float batonAngle = Vector3.SignedAngle(baton.forward, Vector3.up, Vector3.forward);

		float moveDelta = Mathf.DeltaAngle(batonAngle, mouseAngle);
		float moveLength = Mathf.Abs(moveDelta);
		float moveAngle = Mathf.MoveTowardsAngle(batonAngle, mouseAngle, Mathf.Min(maxMove, moveLength * inertia));

		float springDir = Mathf.Clamp(moveDelta * inertia, -maxMove, maxMove);
		if (chargeSpring)
			springCharge += springDir;

		if (!springCharing)
			batonPivot.localRotation = Quaternion.Euler(0, moveAngle, 0);
	}

	public void BatonState(bool extended, float length, float inertia)
	{
		batonExtend = Mathf.Lerp(batonExtend, extended ? length : 0.5f, inertia);
		baton.position = body.position + baton.forward * batonExtend;
		baton.localScale = new Vector3(1, 1, batonExtend);
	}

	public void Move(Vector2 delta)
	{
		rigidBody.AddForce(new Vector2(delta.x, delta.y));
	}

	public void UnleashSpring(float step)
	{
		StartCoroutine(SpringRoutine(step));
	}

	IEnumerator SpringRoutine(float step)
	{
		springCharing = true;
		while (Mathf.Abs(springCharge) > step * 1.5f)
		{
			float springDir = springCharge / Mathf.Abs(springCharge);

			batonPivot.localRotation *= Quaternion.Euler(0, step * -springDir, 0);
			springCharge -= step * springDir;

			yield return new WaitForSeconds(0);
		}

		springCharing = false;
		springCharge = 0;
	}
}
