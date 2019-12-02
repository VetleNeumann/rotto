using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //need to be add to library


public class Menucontroller : MonoBehaviour
{

public string MainMenuScene;
public Gameobject PauseMenu;
public bool IsPaused;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetkeyDown(KeyCode.Escape))
		{
			if (IsPaused)
				{
				   Resume ();
				}
			else
			    {
					IsPaused=true;
					PauseMenu.SetActive(true);
					Time.timeScale=0f;


				}
		}
		
	 }

	public void Resume ()
	{
					IsPaused=false;
					PauseMenu.SetActive(false);
					Time.timeScale=1f;
	
	}


	public void ReturnToMain()
	{
		Time.timeScale=1f;
		SceneManager.LoadScene(MainMenuScene);

	
	}


}
