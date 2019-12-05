using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundwaveManager : MonoBehaviour
{
    [SerializeField]
    float startSpeed = 0f;

    float speed;

    [SerializeField]
    float maxSpeed = 7f;

    [SerializeField]
    float accelRate = 0.5f;
    [SerializeField]
    float expAccelRate = 0.5f;

    [SerializeField]
    float growthRate = 1.001f;

    [SerializeField]
    float lifeTime = 7f;

    // Start is called before the first frame update
    void Start()
    {
        speed = startSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        scale();

        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            Destroy(gameObject);
    }

    void move()
    {
        if (speed < 1) { 
            speed = Mathf.Lerp(speed, 2, accelRate);
        }
        else
        {
            speed *= expAccelRate;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
            transform.position = transform.position + (transform.up * speed * Time.deltaTime);
        }
        
    }

    void scale()
    {
        Vector3 scale = transform.localScale;
        scale = new Vector3(scale.x * growthRate, scale.y, scale.z * growthRate);
        transform.localScale = scale;
    }
}
