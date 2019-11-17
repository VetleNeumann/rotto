using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnstileController : ControllerBase
{
    bool firstPress = false;

    Rigidbody rigidBody;
    BoxCollider batonCollider;

    [SerializeField]
    Material red, green;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void ButtonPressed()
    {
        //FreezeRotation(false);
        if (!firstPress)

        StartCoroutine(ButtonToggled());
        firstPress = true;
    }

    IEnumerator ButtonToggled()
    {
        Transform turnstile = GameObject.Find("Turnstile").transform;
        MeshRenderer button = new MeshRenderer();
        Light light = new Light();
        foreach (Transform child in transform)
        {
            switch (child.tag)
            {
                case "Button":
                    button = child.GetComponent<MeshRenderer>();
                    print("found");
                    break;
                case "Light":
                    light = child.GetComponent<Light>();
                    break;
                default:
                    break;
            }
        }

        if (button == new MeshRenderer())
        {
            Debug.LogError("Couldn't find button.");
            yield break;
        }
        else if (light == new Light())
        {
            Debug.LogError("Couldn't find light.");
            yield break;
        }

        yield return new WaitForSeconds(0.3f);
        //Shut off light
        light.enabled = false;
        yield return new WaitForSeconds(0.8f);
        //Turn light green
        light.color = Color.green;
        button.material = green;
        yield return new WaitForSeconds(0.35f);
        //Turn light on
        light.enabled = true;
        //Play sound

        //Unfreeze turnstile
        turnstile.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationY;
    }
}
