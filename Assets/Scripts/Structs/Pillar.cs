using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pillar
{
    public Pillar(Transform transform, ColorRGB color)
    {
        this.transform = transform;
        this.color = color;
        this.activated = false;
    }

    public Transform transform;
    public ColorRGB color;
    public bool activated;
}
