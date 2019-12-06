using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    BossController bossController;
    BossManager bossManager;

    void Start()
    {
        bossController = GameObject.Find("Boss").GetComponent<BossController>();
        bossManager = GameObject.Find("Boss").GetComponent<BossManager>();
    }
    void OnTriggerEnter()
    {
        if (bossController.CoreExposed())
            bossController.CoreHit();
    }
}
