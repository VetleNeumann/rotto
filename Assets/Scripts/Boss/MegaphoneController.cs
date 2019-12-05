using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MegaphoneManager))]
public class MegaphoneController : MonoBehaviour
{
    MegaphoneManager manager;

    bool deployed = false;

    [SerializeField]
    float upLength = 10f;
    [SerializeField]
    float downLength = 0.5f;

    [SerializeField]
    float turnRate = 0.5f;

    [SerializeField]
    float spawnDistance = -1;

    [SerializeField]
    GameObject soundwave;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<MegaphoneManager>();

        transform.position = transform.position + Vector3.up * upLength;
    }

    // Update is called once per frame
    void Update()
    {
        manager.turnTowardsRotto(turnRate);

        if (Input.GetKeyDown(KeyCode.Space))
            SetDeployedState(!deployed);
        if (Input.GetKeyDown(KeyCode.LeftShift))
            SpawnSoundWave();
    }

    void SpawnSoundWave()
    {
        manager.SpawnSoundwave(soundwave, spawnDistance);
    }

    void SetDeployedState(bool deploy)
    {
        if (deploy == deployed)
            return;

        if (manager.coroutineRunning)
            StopCoroutine(co);

        if (deploy)
            co = StartCoroutine(manager.deployMegaphone(upLength));
        else
            co = StartCoroutine(manager.liftMegaphone(downLength, upLength));
        deployed = deploy;
    }
}
