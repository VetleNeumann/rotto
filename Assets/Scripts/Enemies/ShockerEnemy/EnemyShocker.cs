using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Timers;

public class EnemyShocker : BaseEnemy
{
    MeshRenderer[] MaterialZone;
    private static System.Timers.Timer clock;
    bool ShockOn = false;
    public float timer = 4f;
    public float state = 0f;
    Renderer[] rend;
    Transform[] tagg;
    Transform[] scale;
    public Transform target;
    float size = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        SetRender(false);
        SetGametag("Untagged");
    }

    // Update is called once per frame
    void Update()
    {
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //agent.destination = target.position;
        timer -= Time.deltaTime;
        if (timer <= 3f && state == 0f)
        {
            SetRender(false);
            SetGametag("Untagged");

            state = 2f;
        }
        if (timer <= 1f && state == 1f)
        {
            SetRender(true);

            SetMaterial(Color.yellow);
            state = 2f;
        }
        if (timer <= 0.5f && state == 2f)
        {
            SetRender(true);
            SetGametag("dmgObject");
            SetMaterial(Color.red);
            
            state = 3f;
            

        }
        if (timer <= 0f && state == 3f)
        {
            state = 0f;
            timer = 3f;
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



    }

    void SetRender(bool state)
    {
        rend = GetComponentsInChildren<Renderer>();
        foreach (Renderer Zone in rend)
            if (Zone.name == "ShockZone")
            {
                Zone.enabled = state;
            }
    }
    void SetGametag(string tag)
    {
        tagg = GetComponentsInChildren<Transform>();
        foreach (Transform Zone in tagg)
            if (Zone.name == "ShockZone")
            {
                transform.gameObject.tag = tag;
            }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Baton Pivot")
        {
            Destroy(gameObject);
            Room.RemoveEnemy(this);
        }
    }
    public override void SetTarget(PlayerController target)
    {
        Transform goal= this.target;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(goal.position);
    }
     //not working
    IEnumerator ZoneScale()
    {
        scale = GetComponentsInChildren<Transform>();
        foreach (Transform Zone in scale)
            if (Zone.name == "ShockZone")
            {
               
                while( size < 4f)
                {
                    
                    //Zone.transform.localScale = new Vector3(size, 5f, size);
                    transform.localScale = new Vector3(size, 5f, size);
                    size += 1f;
                    yield return new WaitForSeconds(1f);
                }
                       
            }


    }
}
