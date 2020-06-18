using UnityEngine;
using UnityEngine.SceneManagement;

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