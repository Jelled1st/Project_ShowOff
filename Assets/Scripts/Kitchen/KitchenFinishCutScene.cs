﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class KitchenFinishCutScene : MonoBehaviour, IGameHandlerObserver
{
    [SerializeField] KitchenGameHandler _gameHandler;
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _cameraNode;
    [SerializeField] GameObject _scoreUI;
    [SerializeField] float _tweenTime = 1.0f;
    [SerializeField] string _nextScene;
    [SerializeField] GameObject _completedDishNode;
    bool _gamefinished = false;

    // Start is called before the first frame update
    void Start()
    {
        Subscribe(_gameHandler);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gamefinished)
        {
            if (!DOTween.IsTweening(_camera.transform))
            {
                if (!_scoreUI.activeSelf)
                {
                    _scoreUI.SetActive(true);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(LoadScene());
                }
            }
        }
    }

    public void OnContinue()
    {
    }

    public void OnFinish()
    {
        _gamefinished = true;
        _camera.transform.DOMove(_cameraNode.transform.position, _tweenTime);
        _camera.transform.DORotate(_cameraNode.transform.rotation.eulerAngles, _tweenTime);
        CopyAndPlaceDish();
    }

    private void CopyAndPlaceDish()
    {
        GameObject dish = Instantiate(_gameHandler.GetChosenDish().transform.parent.gameObject);
        Destroy(dish.GetComponent<Dish>());
        dish.transform.SetParent(_completedDishNode.transform);
        dish.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
    }

    public void OnPause()
    {
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load specified scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_nextScene);
    }
}