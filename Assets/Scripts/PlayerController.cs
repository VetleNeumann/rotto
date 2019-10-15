using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerController : MonoBehaviour
{
    PlayerManager playerManager;
    CameraManager cameraManager;
    
	[SerializeField]
    float accelrate;

	[SerializeField]
	float batonRotateInertia;

	[SerializeField]
	float batonStateInertian;

	[SerializeField]
	float batonMaxMove;

	[SerializeField]
	float batonExtendLength;

	// Start is called before the first frame update
	void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        BatonControl();
        MovementControl();
    }

    void BatonControl()
    {
		bool extendBaton = Input.GetMouseButton(0);
		bool chargeSpring = Input.GetMouseButton(1);

		if (Input.GetMouseButtonUp(1))
			playerManager.UnleashSpring(3);

		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		playerManager.BatonRotate(mouseWorldPos, batonMaxMove, batonRotateInertia, chargeSpring);

		playerManager.BatonState(extendBaton, batonExtendLength, batonStateInertian);
	}

    void MovementControl()
    {
        Vector2 inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        playerManager.Move(inputs * accelrate);
    }
}
