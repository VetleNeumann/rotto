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

	Vector3 batonStartPos;
	float batonExtend;

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
					batonStartPos = baton.position;
                    break;
                case "Body":
                    body = child;
                    break;
                default:
                    break;
            }
        }
    }

    public void BatonRotate(Vector3 position, float maxMove, float inertia)
    {
		float mouseAngle = Vector3.SignedAngle(position - transform.position, Vector3.up, Vector3.forward);
		float batonAngle = Vector3.SignedAngle(baton.forward, Vector3.up, Vector3.forward);

		float moveDelta = Mathf.Abs(Mathf.DeltaAngle(batonAngle, mouseAngle));
		float moveAngle = Mathf.MoveTowardsAngle(batonAngle, mouseAngle, Mathf.Min(maxMove, moveDelta * inertia));

		//Debug.Log($"Mouse: {mouseAngle}, Baton: {batonAngle}, Delta: {moveDelta}, Lerped: {moveAngle}");
		batonPivot.localRotation = Quaternion.Euler(0, moveAngle, inertia);
    }

	public void BatonState(bool extended, float length, float inertia)
	{
		batonExtend = Mathf.Lerp(batonExtend, extended ? length : 0f, inertia);
		baton.position = batonStartPos + baton.forward * batonExtend;
		baton.localScale = new Vector3(1, 1, batonExtend);
	}

    public void Move(Vector2 delta)
    {
        rigidBody.AddForce(new Vector2(delta.x, delta.y));
    }
}
