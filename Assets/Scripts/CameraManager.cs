using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject rotto;
    public float yOffset = 11f;
    public float zOffset = 0f;
    public float xRotation = 80f;

    // Start is called before the first frame update
    void Start()
    {
        rotto = GameObject.FindGameObjectWithTag("Player");
    }

    void OnValidate()
    {
        transform.rotation = Quaternion.Euler(new Vector3(xRotation, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = rotto.transform.position + new Vector3(0, yOffset, zOffset);
    }
}
