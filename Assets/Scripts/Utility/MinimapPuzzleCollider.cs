using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPuzzleCollider : MonoBehaviour
{
    MinimapPuzzleController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<MinimapPuzzleController>();
    }

    void OnCollisionEnter()
    {
        controller.ButtonPressed(transform.name);
    }
}
