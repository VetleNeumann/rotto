using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    Transform keyModel;
    Vector3 initialPos;
    //Quaternion initialRot;
    float spinTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "model":
                    keyModel = child;
                    break;
                default:
                    break;
            }
        }

        initialPos = keyModel.localPosition;
        //initialRot = keyModel.rotation;
    }

    public void DeleteKey()
    {
        gameObject.SetActive(false);
    }

    public void Spin(float horSpeed, float verSpeed, float verHeight)
    {
        Quaternion currentRot = keyModel.localRotation;
        keyModel.localRotation = Quaternion.Slerp(currentRot, Quaternion.Euler(currentRot.eulerAngles + Vector3.up), horSpeed * Time.deltaTime);

        Vector3 pos = keyModel.localPosition;
        spinTimer += Time.deltaTime;
        if (spinTimer > verSpeed)
            spinTimer -= verSpeed;
        float periodic = Mathf.Sin(spinTimer * (2 * Mathf.PI) / verSpeed);
        keyModel.localPosition = new Vector3(pos.x, initialPos.y + periodic * verHeight, pos.z);
    }
}
