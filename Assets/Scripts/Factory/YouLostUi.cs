using UnityEngine;
using UnityEngine.SceneManagement;

public class YouLostUi : MonoBehaviour
{
    [SerializeField]
    private string mainMenuName = "!Main Menu";

    public void ReturnToMain()
    {
        SceneManager.LoadScene(mainMenuName);
    }

    public void RestartLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }
}