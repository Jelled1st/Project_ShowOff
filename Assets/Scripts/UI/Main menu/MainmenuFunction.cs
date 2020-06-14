using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuFunction : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("!Farm Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}