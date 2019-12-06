using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonCollider : MonoBehaviour
{
    [SerializeField]
    Collider pivotCounterweight;

	private void OnCollisionEnter(Collision collision)
	{
        Collider thisCollider = collision.GetContact(0).thisCollider;
        if (thisCollider == pivotCounterweight)
        {
            Physics.IgnoreCollision(thisCollider, collision.collider);
            return;
        }

        if (collision.collider.tag.Equals("Player"))
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collision.collider);

        //collision.gameObject.GetComponent<BaseEnemy>()?.Hit(1);
    }
}
