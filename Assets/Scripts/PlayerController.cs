using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerController : MonoBehaviour
{
    PlayerManager playerManager;
    CameraManager cameraManager;
    
    float accelrate = 52f;

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
        Vector2 mousePosition = Input.mousePosition;
        //Calculates the relative position of the mouse cursor, which can be anywhere
        //between (-1,-1) (bottom left), or (1,1) (upper right).
        Vector2 relativeMousePosition = 2f * new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height) - Vector2.one;
        //Limits the maximum and minimum value of the mouse position in case the cursor
        //is outside the screen.
        relativeMousePosition = new Vector2(Mathf.Clamp(relativeMousePosition.x, -1f, 1f), Mathf.Clamp(relativeMousePosition.y, -1f, 1f));

        //Calculates the angle about the center.
        Quaternion mouseCenterAngle = Quaternion.Euler(0f, Vector2.SignedAngle(Vector2.down, relativeMousePosition), 0f);
        mouseCenterAngle.x *= -1f;
        mouseCenterAngle.w *= -1f;

        playerManager.BatonRotate(mouseCenterAngle);
    }

    void MovementControl()
    {
        Vector2 inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        playerManager.Move(inputs * accelrate);
    }
}
