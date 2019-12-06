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
        if (Vector3.SqrMagnitude(transform.position - (collider.transform.position - collider.transform.forward)) <
            Vector3.SqrMagnitude(transform.position - (collider.transform.position + collider.transform.forward)))
            return;
		puzzleController.PillarHit(transform);
	}
}
