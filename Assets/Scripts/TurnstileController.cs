using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnstileController : ControllerBase
{
    Rigidbody2D rigidBody;
    BoxCollider2D batonCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        FreezeRotation(true);
    }

    void FreezeRotation(bool freeze)
    {
        rigidBody.freezeRotation = freeze;
    }

    public override void ButtonPressed()
    {
        FreezeRotation(false);
    }
}
