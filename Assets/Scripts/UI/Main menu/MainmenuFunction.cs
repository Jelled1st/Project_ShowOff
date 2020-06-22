using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuFunction : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("!Farm Level");
    }

    public void CreditScene()
    {
        SceneManager.LoadScene("!Credits");
    }

    public void ScoreScene()
    {
        ScoreSceneController.ShowInput = false;
        SceneManager.LoadScene("!ScoreInput");
    }
}