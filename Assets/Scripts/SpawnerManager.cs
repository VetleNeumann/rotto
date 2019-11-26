using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy(GameObject prefab)
    {
        //Spawn enemy
        Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity, transform.parent);
        //Maybe add animation of enemy coming up from ground
    }
}
