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
    Rigidbody batonRigidBody;
	Projector projector;

	float batonExtend;
	float springCharge;
	bool springCharging = false;

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
                //Baton Pivot is the gameobject with colliders and a rigidbody.
				case "Baton Pivot":
					batonPivot = child;
					baton = batonPivot.GetChild(0).GetComponent<Transform>();
					break;
				case "Body":
					body = child;
                    print(body.name);
					break;
				default:
					break;
			}
		}

        batonRigidBody = batonPivot.GetComponent<Rigidbody>();

        //batonRigidBody.AddTorque(new Vector3(0, 10, 0), ForceMode.VelocityChange);
        TurnOnLights();
    }

	public void BatonRotate(Ray cameraRay, float maxMove, float inertia, bool chargeSpring)
	{

		Vector3 position = GetMousePos(cameraRay);

		float mouseAngle = Vector3.SignedAngle(position - new Vector3(transform.position.x, 0, transform.position.z), Vector3.forward, Vector3.down);
        //float batonAngle = Vector3.SignedAngle(batonPivot.rotation.eulerAngles, Vector3.up, Vector3.up);
        float batonAngle = batonPivot.rotation.eulerAngles.y;
        if (batonAngle >= 360)
            batonAngle -= Mathf.Repeat(360f, batonAngle);
        if (batonAngle > 180)
            batonAngle -= 360;

		float moveDelta = Mathf.DeltaAngle(batonAngle, mouseAngle);
		float moveLength = Mathf.Abs(moveDelta);
		float moveAngle = Mathf.MoveTowardsAngle(batonAngle, mouseAngle, Mathf.Min(maxMove, moveLength * inertia));

        float springDir = moveDelta * inertia;
		//springDir = Mathf.Clamp(moveDelta * inertia, -maxMove, maxMove);
        if (chargeSpring)
            springCharge += Mathf.Clamp(springDir, -maxMove, maxMove);

        else if (!chargeSpring)
        {
            if (Mathf.Abs(springDir) < maxMove)
            {
                //Deaccelerate
                float currentDirection = batonRigidBody.angularVelocity.y;
                batonRigidBody.AddTorque(0f, -5 * batonRigidBody.angularVelocity.y/springDir, 0f);
                if (currentDirection != Mathf.Sign(batonRigidBody.angularVelocity.y))
                    batonRigidBody.angularVelocity = Vector3.zero;
            }
            else
            {
                //Accelerate / Clamp speed
                batonRigidBody.AddTorque(new Vector3(0f, springDir, 0f));

                Vector3 angVel = batonRigidBody.angularVelocity;
                batonRigidBody.angularVelocity = new Vector3(angVel.x, Mathf.Clamp(angVel.y, -maxMove, maxMove), angVel.z);
            }
        }
        
        //batonPivot.localRotation = Quaternion.Euler(0, batonPivot.localRotation.eulerAngles.x, 0);
        //baton.localRotation = Quaternion.Euler(90, 0, 0);
    }

	public void BatonState(bool extended, float retractedLength, float extendedLength, float inertia)
	{
		batonExtend = Mathf.Lerp(batonExtend, extended ? extendedLength : retractedLength, inertia);
		//baton.position = body.position + batonPivot.forward * batonExtend + batonPivot.forward / 2f;
		batonPivot.localScale = new Vector3(0.25f, 0.25f, batonExtend);
	}

    public void Move(Vector2 inputs, float maxSpeed, float accelRate)
    {
        Vector3 deltaForce = new Vector3(inputs.x, 0, inputs.y) * accelRate;
        if (inputs.Equals(Vector2.zero))
            deltaForce = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z) * -accelRate * 0.25f;

        rigidBody.AddForce(deltaForce);
        if (rigidBody.velocity.magnitude > maxSpeed)
            rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
    }

    public void TurnBody(float turnRate)
    {
        float velocityAngle = Vector3.SignedAngle(Vector3.forward, rigidBody.velocity, Vector3.up);
        body.localRotation = Quaternion.Slerp(body.localRotation, Quaternion.Euler(0, velocityAngle, 0), turnRate);
    }

    public void UnleashSpring(float step)
	{
		StartCoroutine(SpringRoutine(step));
	}

	public void ToggleMinimap(bool state)
	{
		if (projector != null)
			projector.enabled = state;
	}

	Vector3 GetMousePos(Ray cameraRay)
	{
		Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Ground"));
		return hit.point;
	}

	IEnumerator SpringRoutine(float step)
	{
		springCharging = true;
		while (Mathf.Abs(springCharge) > step * 1.5f)
		{
			float springDir = springCharge / Mathf.Abs(springCharge);

			batonPivot.localRotation *= Quaternion.Euler(0, step * -springDir, 0);
			springCharge -= step * springDir;

			yield return new WaitForSeconds(0);
		}

		springCharging = false;
		springCharge = 0;
	}

    public void ChangeLightLevel(float lightLevel)
    {
        if (lightLevel == 0f)
        { Changelight("light1", false); Changelight("light2", false); Changelight("light3", false); Changelight("light4", false);//Debug.Log("l0"); 
        }
        if (lightLevel==1f)
        { Changelight("light1", true); Changelight("light2", false); Changelight("light3", false); Changelight("light4", false);//Debug.Log("l1"); 
        }
        if (lightLevel == 2f)
        { Changelight("light1", true); Changelight("light2", true); Changelight("light3", false); Changelight("light4", false); //Debug.Log("l2");
        }
        if (lightLevel == 3f)
        { Changelight("light1", true); Changelight("light2", true); Changelight("light3", true); Changelight("light4", false); //Debug.Log("l3"); 
        }
        if (lightLevel == 4f)
        { Changelight("light1", true); Changelight("light2", true); Changelight("light3", true); Changelight("light4", true); //Debug.Log("l4"); 
        }

    }
    public void Changelight(string lightName, bool lightOn)
    {
        MeshRenderer[] lights = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer light in lights)
            if (light.name == (lightName))
                {
                if (lightOn == true)
                    light.material.color = Color.yellow;
                else if (lightOn == false)
                    light.material.color = Color.black;

            }

    }
    public void TurnOnLights()
    {
        ChangeLightLevel(4);
    }

}
