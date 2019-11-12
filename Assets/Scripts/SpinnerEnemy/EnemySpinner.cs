using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpinner : MonoBehaviour
{
    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = target.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(new Vector3(0f, 90f, 0f), 0.1f, Space.Self);
       
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Baton")
        {
            Destroy(gameObject);
        }
    }
}
