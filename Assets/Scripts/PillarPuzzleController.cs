using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PillarPuzzleManager))]
public class PillarPuzzleController : MonoBehaviour
{
    PillarPuzzleManager puzzleManager;
    Pillar[] pillars;

    float initialTime = 1.2f;
    float timer;
    bool timerRunning = false;
    ColorRGB prevColor = ColorRGB.Red;

    [SerializeField]
    DoorController solvedDoor;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GetComponent<PillarPuzzleManager>();

        pillars = new Pillar[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            string name = child.name;
            ColorRGB color = name[0] == 'R' ? ColorRGB.Red : name[0] == 'B' ? ColorRGB.Blue : ColorRGB.Green;
            pillars[i] = new Pillar(child, color);
        }

        timer = initialTime;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        if (timerRunning)
            timer -= Time.deltaTime;

        if (timer <= 0f)
            ResetPuzzle();
    }

    bool DoneWithColor(ColorRGB color)
    {
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i].color == color && !pillars[i].activated)
                return false;
        }
        return true;
    }

    void ResetPuzzle()
    {
        timerRunning = false;
        timer = initialTime;
        for (int i = 0; i < pillars.Length; i++)
            if (pillars[i].activated)
            {
                pillars[i].activated = false;
                puzzleManager.SetPillarStatus(pillars[i], false);
            }
            
    }

    public void PillarHit(Transform pillar)
    {
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillar.transform == pillars[i].transform && !pillars[i].activated)
            {
                if (!timerRunning)
                {
                    timerRunning = true;
                    prevColor = pillars[i].color;
                }

                if (pillars[i].color == prevColor || DoneWithColor(prevColor))
                {
                    pillars[i].activated = true;
                    puzzleManager.SetPillarStatus(pillars[i], true);
                }
                else
                {
                    ResetPuzzle();
                    return;
                }

                if (DoneWithColor(ColorRGB.Red) && DoneWithColor(ColorRGB.Green) && DoneWithColor(ColorRGB.Blue))
                {
                    timerRunning = false;
                    if (solvedDoor != null)
                        solvedDoor.ToggleDoor();
                }
                break;
            }
        }
    }
}
