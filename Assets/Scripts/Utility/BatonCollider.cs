using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonCollider : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.GetComponent<BaseEnemy>()?.Hit(1);
	}
}
