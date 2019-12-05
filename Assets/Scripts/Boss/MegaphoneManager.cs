using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaphoneManager : MonoBehaviour
{
    Transform rotto;
    Transform model;
    Transform claw;

    bool aimedAtRotto = false;
    public bool coroutineRunning { get; private set; } = false;
    Vector3 modelClawPositionDiff;

    // Start is called before the first frame update
    void Start()
    {
        rotto = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "Arm_Closed":
                    claw = child;
                    break;
                case "Model":
                    model = child;
                    break;
                default:
                    break;
            }
        }

        modelClawPositionDiff = claw.position - model.position;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustClawPos();
    }

    void AdjustClawPos()
    {
        claw.position = model.position + modelClawPositionDiff;
    }

    public void turnTowardsRotto(float turnRate)
    {
        Vector3 target = model.localPosition * 2;

        if (aimedAtRotto)
            target = rotto.position;

        Vector3 directionVector = transform.position - target;
        directionVector.y = 0;
        float angleBetween = Vector3.SignedAngle(Vector3.forward, directionVector, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angleBetween, 0), turnRate);


        float heightDifference = transform.position.y - target.y;
        float distanceToRotto = directionVector.magnitude;
        float pitchAngle = Mathf.Atan2(distanceToRotto, heightDifference)*Mathf.Rad2Deg;
        if (!aimedAtRotto)
            pitchAngle = 90f;
        model.localRotation = Quaternion.Slerp(model.localRotation, Quaternion.Euler(pitchAngle - 90, 0, 0), turnRate); 
    }

    public IEnumerator liftMegaphone(float downLength = 0.5f, float upLength = 10f)
    {
        coroutineRunning = true;

        aimedAtRotto = false;
        yield return new WaitForSeconds(0.5f);
        //go down
        float initialY = transform.position.y;
        while(transform.position.y > initialY - downLength * 0.7f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, initialY - downLength, transform.position.z), 0.02f);
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, initialY - downLength, transform.position.z);

        float speed = 0f;
        while (transform.position.y < initialY + upLength * 0.7f)
        {
            speed = Mathf.Lerp(0f, 10f, 0.015f);
            transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, initialY + upLength, transform.position.z);

        coroutineRunning = false;
    }

    public IEnumerator deployMegaphone(float downLength = 10f)
    {
        coroutineRunning = true;

        float initialY = transform.position.y;
        while (transform.position.y > initialY - downLength)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, initialY - downLength * 1.05f, transform.position.z), 0.012f);
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, initialY - downLength, transform.position.z);
        aimedAtRotto = true;

        coroutineRunning = false;
    }

    public void SpawnSoundwave(GameObject soundwavePrefab, float spawnDistance)
    {
        Vector3 modelRot = model.rotation.eulerAngles;
        Instantiate(soundwavePrefab, model.position + model.forward * spawnDistance, Quaternion.Euler(modelRot + Vector3.left * 90));
    }
}
