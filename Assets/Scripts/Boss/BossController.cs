using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossManager))]
public class BossController : MonoBehaviour
{
    BossManager manager;

    [SerializeField]
    GameObject clawPrefab;
    [SerializeField]
    GameObject[] enemyPrefabs = new GameObject[2];

    [SerializeField]
    float layerTurnRate = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<BossManager>();
    }

    // Update is called once per frame
    void Update()
    {
        manager.TurnHead();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            manager.DeployEnemy(clawPrefab, enemyPrefabs[0], new Vector3(-3, 1, -3));
    }

    public void ButtonPushed(int button)
    {
        StartCoroutine(manager.TurnLayer(button, layerTurnRate));
    }
}
