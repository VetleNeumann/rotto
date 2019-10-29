using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pillar
{
	public Transform Transform { get; }

	public ColorRGB Color { get; }

	public bool Activated { get; set; }

	public Pillar(Transform transform, ColorRGB color)
    {
        this.Transform = transform;
        this.Color = color;
        this.Activated = false;
    }
}
