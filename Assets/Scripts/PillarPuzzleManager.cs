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

    public void SetPillarStatus(Pillar pillar, Material material, bool enabled)
    {
		// Needs to be changed to 3D
		Renderer rendere = pillar.Transform.GetComponent<Renderer>();
        Color newColor = new Color();
        switch (pillar.Color)
        {
            case ColorRGB.Red:
                newColor = new Color(colorDiff, 0f, 0f);
                break;
            case ColorRGB.Green:
                newColor = new Color(0f, colorDiff, 0f);
                break;
            case ColorRGB.Blue:
                newColor = new Color(0f, 0f, colorDiff);
                break;
        }

		rendere.material = material;
		/*
        if (enabled)
            rendere.color += newColor;
        else
            rendere.color -= newColor;
			*/
    }
}