using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    HealthManager healthManager = new HealthManager();

    // Start is called before the first frame update
    void Start()
    {
        //healthManager.SetHealth(100);
    }

    // Update is called once per frame
    
    void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "weapon")
		{
			healthManager.AddHealth(1);
		}
	}
}
