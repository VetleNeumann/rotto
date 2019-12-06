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

    [SerializeField]
    int spawnPercentChance;

    Coroutine enemySpawnCoroutine;
    Coroutine projectileCoroutine;

    int bossState = 0;

    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<BossManager>();
        enemySpawnCoroutine = StartCoroutine(SpawnEnemies());
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

    public void CoreHit()
    {
        bossState++;
        StartCoroutine(ResetButtons());
        if (bossState == 1) ;
        if (bossState == 2) ;
    }

    bool CoreExposed()
    {
        for (int i = 0; i < 4; i++)
            if (!manager.layerStates[i])
                return false;
        return true;
    }

    IEnumerator ResetButtons()
    {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 4; ++i)
        {
            manager.ResetButton(Random.Range(0, 4));
            yield return new WaitForSeconds(4);
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true) {
            bool dontSpawn = false;
            if (CoreExposed())
                dontSpawn = true;

            if (!dontSpawn) ;
            else if (Random.Range(0, 100) > (100 - spawnPercentChance))
                manager.DeployEnemy(clawPrefab, enemyPrefabs[Random.Range(0, 2)], new Vector3(Random.Range(-7f, 7f), 1, Random.Range(-6f, 2f)));
            yield return new WaitForSeconds(3f);
        }
    }
}
