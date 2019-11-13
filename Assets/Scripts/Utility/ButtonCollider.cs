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

    void OnCollisionEnter()
    {
        controller.ButtonPressed();
    }
}
