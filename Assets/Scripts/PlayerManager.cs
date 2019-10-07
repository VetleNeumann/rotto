using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Transform batonPivot;
    Transform baton;
    Transform body;

    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BatonRotate(Quaternion angle)
    {
        batonPivot.rotation = angle;
    }

    public void Move(Vector2 delta)
    {
        rigidBody.AddForce(new Vector3(delta.x, 0f, delta.y));
    }
}
