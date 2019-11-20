using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
	Transform batonPivot;
	Transform baton;
	Transform body;

	Rigidbody rigidBody;
	Projector projector;

	float batonExtend;
	float springCharge;
	bool springCharing = false;

	private void Awake()
	{
		rigidBody = gameObject.GetComponent<Rigidbody>();
		projector = gameObject.GetComponentInChildren<Projector>();
	}

	void Start()
	{
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

	public void BatonRotate(Ray cameraRay, float maxMove, float inertia, bool chargeSpring)
	{
		Vector3 position = GetMousePos(cameraRay);

		float mouseAngle = Vector3.SignedAngle(position - new Vector3(transform.position.x, 0, transform.position.z), Vector3.forward, Vector3.down);
		float batonAngle = Vector3.SignedAngle(baton.forward, Vector3.forward, Vector3.down);

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
		baton.position = body.position + baton.forward * batonExtend + baton.forward / 2f;
		baton.localScale = new Vector3(1, 1, batonExtend);
	}

    public void Move(Vector2 delta, float maxSpeed, float accelRate)
    {
        Vector3 deltaMovement = new Vector3(delta.x, 0, delta.y) * accelRate * Time.deltaTime;
        if (deltaMovement.magnitude > maxSpeed * Time.deltaTime)
            deltaMovement = deltaMovement.normalized * maxSpeed * Time.deltaTime;

        transform.Translate(deltaMovement);
    }

    public void UnleashSpring(float step)
	{
		StartCoroutine(SpringRoutine(step));
	}

	public void ToggleMinimap(bool state)
	{
		projector.enabled = state;
	}

	Vector3 GetMousePos(Ray cameraRay)
	{
		Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Ground"));
		return hit.point;
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
