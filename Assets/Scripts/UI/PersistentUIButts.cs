﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentUIButts : MonoBehaviour
{
    [SerializeField]
    private string mainMenuName = "!Main Menu";

    [SerializeField]
    private GameObject mainConfirm;

    [SerializeField]
    private GameObject resetConfirm;

    private void Start()
    {
        if (mainConfirm.activeInHierarchy)
        {
            mainConfirm.SetActive(false);
        }

        if (resetConfirm.activeInHierarchy)
        {
            resetConfirm.SetActive(false);
        }
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(mainMenuName);
    }

    public void RestartLevel()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
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