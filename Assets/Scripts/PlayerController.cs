using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerController : MonoBehaviour
{
    PlayerManager playerManager;
    CameraManager cameraManager;

    public int keys = 0;
    public bool diagonalView = false;

    [SerializeField]
    float turnRate;

    [SerializeField]
    float accelrate;

    [SerializeField]
    float maxSpeed;

	[SerializeField]
	float batonRotateInertia;

	[SerializeField]
	float batonStateInertian;

	[SerializeField]
	float batonMaxMove;

    [SerializeField]
    float batonRetractLength;
    [SerializeField]
	float batonExtendLength;

    bool extendBaton = false;

    // Start is called before the first frame update
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        cameraManager = Camera.main.GetComponent<CameraManager>();
    }
    // Update is called once per frame
    void Update()
    {
        BatonControl();
        MovementControl();
		MinimapControl();
    }

    void BatonControl()
    {
		extendBaton = Input.GetMouseButtonDown(0) ? !extendBaton : extendBaton;
		bool chargeSpring = Input.GetMouseButton(1);

		if (Input.GetMouseButtonUp(1))
			playerManager.UnleashSpring(3);

		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		playerManager.BatonRotate(mouseRay, batonMaxMove, batonRotateInertia, chargeSpring);

		playerManager.BatonState(extendBaton, batonRetractLength, batonExtendLength, batonStateInertian);
	}

    void MovementControl()
    {
        Vector2 inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (diagonalView && (inputs.x != 0 || inputs.y != 0))
        {
            inputs = new Vector2(inputs.x + inputs.y, -inputs.x + inputs.y).normalized * inputs.magnitude;
        }

        playerManager.Move(inputs, accelrate, maxSpeed);
        playerManager.TurnBody(turnRate);
    }

	void MinimapControl()
	{
		playerManager.ToggleMinimap(Input.GetKey(KeyCode.Space));
	}
}
