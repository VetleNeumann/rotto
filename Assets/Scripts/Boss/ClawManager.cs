using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawManager : MonoBehaviour
{
    Animation armAnimation;
    Transform enemy;
    PlayerController rotto;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "Toggler":
                case "Spinner":
                    enemy = child;
                    break;
                default:
                    break;
            }
        }

        armAnimation = transform.GetComponent<Animation>();
        rotto = GameObject.Find("Rotto").GetComponent<PlayerController>();
    }

    public IEnumerator PlaceEnemy(Transform enemy, float heightAbove)
    {
        Transform parent = transform.parent;
        float initialY = parent.position.y;
        while (parent.position.y > initialY - heightAbove)
        {
            parent.position = Vector3.Lerp(parent.position, new Vector3(parent.position.x, initialY - heightAbove * 1.05f, parent.position.z), 0.012f);
            yield return new WaitForEndOfFrame();
        }
        parent.position = new Vector3(parent.position.x, initialY - heightAbove, parent.position.z);

        armAnimation.Play();
        yield return new WaitForSeconds(2.5f);
        enemy.parent = null;
        enemy.GetComponent<BaseEnemy>().SetTarget(rotto);
        
        float speed = 0f;
        while (parent.position.y < initialY + heightAbove * 0.7f)
        {
            speed = Mathf.Lerp(0f, 5f, 0.01f);
            parent.position = new Vector3(parent.position.x, parent.position.y + speed, parent.position.z);
            yield return new WaitForEndOfFrame();
        }

        Destroy(parent.gameObject);
    }


}
