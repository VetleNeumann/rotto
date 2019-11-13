using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // Start is called before the first frame update
    //HealthController healthController;

    public int HP { get; private set; }

    void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("test");
    }

    public void SetHealth(int SetHpTo)
    {
        HP = SetHpTo;
        //Debug.Log("Hp Set");
        PrintHealth();
    }

    public void AddHealth(int HpToAdd)
    {
        HP += HpToAdd;
        //Debug.Log("Health changed by" + HpToAdd);
        PrintHealth();
    }

    public void PrintHealth()
    {
        //Debug.Log("CurrentHealth" + HP);
    }
}
