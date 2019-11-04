using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	[SerializeField]
	PillarColor[] pillarColors;

	Dictionary<ColorRGB, PillarColor> colorToPillar;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GetComponent<PillarPuzzleManager>();
		colorToPillar = pillarColors.ToDictionary(x => x.Color);

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

    bool DoneWithMaterial(ColorRGB color)
    {
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i].Color == color && !pillars[i].Activated)
                return false;
        }
        return true;
    }

    void ResetPuzzle()
    {
        timerRunning = false;
        timer = initialTime;
        for (int i = 0; i < pillars.Length; i++)
            if (pillars[i].Activated)
            {
                pillars[i].Activated = false;
                puzzleManager.SetPillarStatus(pillars[i], colorToPillar[pillars[i].Color], false);
            }
            
    }

    public void PillarHit(Transform pillar)
    {
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillar.transform == pillars[i].Transform && !pillars[i].Activated)
            {
                if (!timerRunning)
                {
                    timerRunning = true;
                    prevColor = pillars[i].Color;
                }

                if (pillars[i].Color == prevColor || DoneWithMaterial(prevColor))
                {
                    pillars[i].Activated = true;
                    puzzleManager.SetPillarStatus(pillars[i], colorToPillar[pillars[i].Color], true);
                }
                else
                {
                    ResetPuzzle();
                    return;
                }

                if (DoneWithMaterial(ColorRGB.Red) && DoneWithMaterial(ColorRGB.Green) && DoneWithMaterial(ColorRGB.Blue))
                {
                    timerRunning = false;
					if (solvedDoor != null && !solvedDoor.Locked)
						solvedDoor.Solve();
					else
						ResetPuzzle();
                }
                break;
            }
        }
    }
}
