using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField]
    int sceneIndex;

    void OnTriggerEnter()
    {
        GameObject.Find("SceneChanger").GetComponent<SceneChanger>().FadeToLevel(sceneIndex);
    }
}
