using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossButtonController : ControllerBase
{
    BossController boss;
    int buttonNumber;

    [SerializeField]
    Material greenMat;
    [SerializeField]
    Material blackMat;

    Renderer buttonRenderer;
    Light buttonLight;

    bool buttonEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.Find("Boss").GetComponent<BossController>();
        buttonNumber = int.Parse(gameObject.name[gameObject.name.Length - 1].ToString());

        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "Point Light":
                    buttonLight = child.GetComponent<Light>();
                    break;
                case "Cylinder (2)":
                    buttonRenderer = child.GetComponent<Renderer>();
                    break;
                default:
                    break;
            }
        }
    }

    public override void ButtonPressed()
    {
        if (!buttonEnabled)
            return;

        buttonEnabled = false;
        boss.ButtonPushed(buttonNumber);
        StartCoroutine(ChangeColor());
    }

    public void Reset()
    {

    }

    IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(0.2f);
        buttonRenderer.material = blackMat;
        buttonLight.enabled = false;
    }
}
