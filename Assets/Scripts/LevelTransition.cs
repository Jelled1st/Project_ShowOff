using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    [SerializeField]
    private string _nextScene = "";

    private bool messagePassed = false;
    public GameObject blackOutSquare;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void Awake()
    {
        if (_nextScene == "")
        {
            _nextScene = SceneManager.GetActiveScene().name;
            Debug.LogWarning("Next scene not set, reloading this scene assumed");
        }

        //StartCoroutine(FadeIn());
    }

    public void transitionAnim(string message)
    {
        if (message.Equals("end"))
        {
            messagePassed = true;
        }
    }


    private IEnumerator LoadScene(int fadeSpeed = 5)
    {
        yield return null;

        var objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        while (blackOutSquare.GetComponent<Image>().color.a > 0)
        {
            fadeAmount = objectColor.a - fadeSpeed * Time.deltaTime;

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
            yield return null;
        }

        //Begin to load specified scene
        var asyncLoad = SceneManager.LoadSceneAsync(_nextScene);

        //Don't let the scene activate until end message received
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            print("Loading progress: " + asyncLoad.progress * 100 + "%");
            if (asyncLoad.progress >= 0.9f)
            {
                if (messagePassed == true)
                {
                    //fade level out first
                    while (blackOutSquare.GetComponent<Image>().color.a < 1)
                    {
                        fadeAmount = objectColor.a + fadeSpeed * Time.deltaTime;

                        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                        blackOutSquare.GetComponent<Image>().color = objectColor;
                        yield return null;
                    }

                    //activate next scene
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}