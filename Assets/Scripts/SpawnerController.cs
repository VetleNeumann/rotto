using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnerManager))]
public class SpawnerController : MonoBehaviour
{
    SpawnerManager manager;

    [SerializeField]
    GameObject mobPrefab;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SpawnerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
