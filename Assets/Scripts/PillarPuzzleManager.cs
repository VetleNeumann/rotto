using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarPuzzleManager : MonoBehaviour
{
    float colorDiff = 0.3137255f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPillarStatus(Pillar pillar, PillarColor pillarColor, bool enabled)
    {
		// Needs to be changed to 3D
		MeshRenderer renderer = pillar.Transform.GetComponent<MeshRenderer>();

		if (enabled)
			renderer.materials = new[] { renderer.materials[0], pillarColor.After, renderer.materials[2] };
		else
			renderer.materials = new[] { renderer.materials[0], pillarColor.Before, renderer.materials[2] };
	}
}