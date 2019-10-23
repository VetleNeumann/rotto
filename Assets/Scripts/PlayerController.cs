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

    bool extendBaton = false;

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
		extendBaton = Input.GetMouseButtonDown(0) ? !extendBaton : extendBaton;
		bool chargeSpring = Input.GetMouseButton(1);

		if (Input.GetMouseButtonUp(1))
			playerManager.UnleashSpring(3);

		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		playerManager.BatonRotate(mouseRay, batonMaxMove, batonRotateInertia, chargeSpring);

		playerManager.BatonState(extendBaton, batonExtendLength, batonStateInertian);
	}

    void MovementControl()
    {
        Vector2 inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        playerManager.Move(inputs * accelrate);
    }
}
