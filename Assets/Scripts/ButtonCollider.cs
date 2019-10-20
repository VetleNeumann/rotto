using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCollider : MonoBehaviour
{
    DoorController doorController;
    // Start is called before the first frame update
    void Start()
    {
        doorController = GetComponentInParent<DoorController>();
    }

    void OnCollisionEnter2D()
    {
        doorController.ButtonPressed();
    }
}
