using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    Animator animator;

    int levelToLoad;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
