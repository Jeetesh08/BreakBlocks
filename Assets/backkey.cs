using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backkey : MonoBehaviour
{
    public GameObject BackButtonConfirmationWin;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        backButton();
    }
    public void yesButton()
    {
        SceneManager.LoadScene(0);
    }
    public void noButton()
    {
        BackButtonConfirmationWin.SetActive(false);
    }
    void backButton()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
                else if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
                else if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Time.timeScale = 0;
                    BackButtonConfirmationWin.SetActive(true);
                }
            }
        }
    }
}
