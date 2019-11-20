using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pausedText;
    public Button unpauseButton;
    public Button quitToWindowsButton;

    // Start is called before the first frame update
    void Start()
    {
        Button unpauseBtn = unpauseButton.GetComponent<Button>();
        Button quitToWindowsBtn = quitToWindowsButton.GetComponent<Button>();
        unpauseBtn.onClick.AddListener(unpause);
        quitToWindowsBtn.onClick.AddListener(quitToWindows);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (Time.timeScale == 1.0f) 
            {
                //Time.timeScale = 0.0f;
                //pausedText.SetActive(true);
                pause();
                Debug.Log("pause");
            }
           else 
            {
                //Time.timeScale = 1.0f;
                //pausedText.SetActive(false);
                unpause();
                Debug.Log("unpause");
            }
       
        }
    }
    void pause()
    {
        Time.timeScale = 0.0f;
        pausedText.SetActive(true);
    }
   void unpause()
    {
        Time.timeScale = 1.0f;
        pausedText.SetActive(false);
    }
    void quitToWindows()
    {
        Debug.Log("Quit");
        //Application.Quit();
     
    }
}
