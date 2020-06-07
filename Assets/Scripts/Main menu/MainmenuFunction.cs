using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuFunction : MonoBehaviour
{

    public AudioSource buttonPress;
      

    public void PlayGame()
    {
        buttonPress.Play();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}