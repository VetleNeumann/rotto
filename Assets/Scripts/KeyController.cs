using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KeyManager))]
public class KeyController : MonoBehaviour
{
    KeyManager keyManager;
    PlayerController playerController;

    [SerializeField]
    float horSpeed = 30f;
    [SerializeField]
    float verSpeed = 1f;
    [SerializeField]
    float verHeight = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        keyManager = GetComponent<KeyManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Baton Pivot")
        {
            playerController.keys++;
            keyManager.DeleteKey();
        }
    }

    // Update is called once per frame
    void Update()
    {
        keyManager.Spin(horSpeed, verSpeed, verHeight);
    }
}
