using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Timers;

public class EnemyShocker : MonoBehaviour
{
    public MeshRenderer[] MaterialZone;
    private static System.Timers.Timer clock;
    bool ShockOn = false;
    public float timer = 2f;
    public float state = 0f;

    // Start is called before the first frame update
    void Start()
    {
       


    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 10f && state == 0f)
        {
            SetMaterial(Color.green);
            state = 1f; 
        }
        if (timer <= 3f && state == 1f)
        {
            SetMaterial(Color.yellow);
            state = 2f;
        }
        if (timer <= 1f && state == 2f)
        {
            SetMaterial(Color.red);
            state = 3f;
            
        }
        if (timer <= 0f && state == 3f)
        {
            state = 0f;
            timer = 10f;
        }





    }

    

     
    
    void SetMaterial(Color colors)
    {
        MaterialZone = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer Zone in MaterialZone)
            if (Zone.name == "ShockZone")
            {
                Zone.material.color = colors;
            }


        //MeshRenderer i = this.GetComponent<MeshRenderer>();
        //i.material.color = Color.blue;

    }
}
