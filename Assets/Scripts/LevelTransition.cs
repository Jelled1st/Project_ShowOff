using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField]
    private string _nextScene = "";

    [SerializeField]
    private string _previousScene = "";

    private bool messagePassed = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Awake()
    {
        if (_nextScene == "")
        {
            _nextScene = SceneManager.GetActiveScene().name;
            Debug.LogWarning("Next scene not set, reloading this scene assumed");
        }
    }

    public void transitionAnim(String message)
    {
        if (message.Equals("end"))
        {
            messagePassed = true;
        }
    }


    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load specified scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_nextScene);
    }
}