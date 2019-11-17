using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPuzzleController : MonoBehaviour
{
    MeshRenderer[] buttons;
    Light[] lights;

    bool cooldown = false;
    bool[] status = { false, false, true, false, true, true, false, false };
    bool[] goal = { true, false, true, true, false, true, true, false };

    [SerializeField]
    DoorController solvedDoor;

    [SerializeField]
    Material green, red, solved;

    void Start()
    {
        buttons = new MeshRenderer[status.Length];
        lights = new Light[status.Length];

        foreach (Transform child in transform)
        {
            if (child.tag == "Button")
            {
                int index = int.Parse(child.name[child.name.Length - 1].ToString()) - 1;
                buttons[index] = child.GetComponent<MeshRenderer>();
            }
            else if (child.tag == "Light")
            {
                int index = int.Parse(child.name[child.name.Length - 1].ToString()) - 1;
                lights[index] = child.GetComponent<Light>();
            }
        }

        for (int i = 0; i < status.Length; i++)
        {
            buttons[i].material = status[i] ? green : red;
            lights[i].color = status[i] ? Color.green : Color.red;
        }
    }

    public void ButtonPressed(string buttonName)
    {
        print(buttonName);

        if (cooldown)
            return;

        int index = int.Parse(buttonName[buttonName.Length - 1].ToString()) - 1;
        StartCoroutine(BlinkButton(index));
    }

    IEnumerator BlinkButton(int index)
    {
        cooldown = true;
        status[index] = !status[index];
        yield return new WaitForSeconds(0.1f);
        //Turn off light
        lights[index].enabled = false;
        yield return new WaitForSeconds(0.2f);
        //Change color
        buttons[index].material = status[index] ? green : red;
        lights[index].color = status[index] ? Color.green : Color.red;
        yield return new WaitForSeconds(0.1f);
        //Turn on light
        lights[index].enabled = true;
        yield return new WaitForSeconds(0.1f);
        cooldown = false;

        //Check answer
        //(I know this should preferably be in the
        //controller but i dont know how to make it work nicely
        //and it doesnt really matter that this puzzle is
        //flexible because its a one time thing)
        StartCoroutine(CheckAnswer());
    }

    IEnumerator CheckAnswer()
    {
        for (int i = 0; i < status.Length; i++)
        {
            if (status[i] != goal[i])
                yield break;
        }

        cooldown = true;
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < status.Length; i++)
        {
            buttons[i].material = solved;
            lights[i].enabled = false;
        }

        if (solvedDoor != null)
            solvedDoor.Solve();
    }
}
