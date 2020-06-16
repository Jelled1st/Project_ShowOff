using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{

    [SerializeField] private string _nextScene = "";

    bool messagePassed = false;

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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_nextScene);

        //Don't let the scene activate until end message received
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if(asyncOperation.progress >= 0.9f)
            {
                if (messagePassed == true)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
