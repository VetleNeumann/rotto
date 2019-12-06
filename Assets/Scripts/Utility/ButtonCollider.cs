using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCollider : MonoBehaviour
{
    ControllerBase controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<ControllerBase>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Vector3.SqrMagnitude(transform.position - (collision.collider.transform.position - collision.collider.transform.forward)) <
            Vector3.SqrMagnitude(transform.position - (collision.collider.transform.position + collision.collider.transform.forward)))
            return;
        controller.ButtonPressed();
    }
}
