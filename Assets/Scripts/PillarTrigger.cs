using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarTrigger : MonoBehaviour
{
    PillarPuzzleController puzzleController;
    
    void Start()
    {
        puzzleController = GetComponentInParent<PillarPuzzleController>();
    }

	void OnTriggerEnter(Collider collider)
	{
		puzzleController.PillarHit(transform);
	}
}
