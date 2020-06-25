using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class KitchenFinishCutScene : MonoBehaviour, IGameHandlerObserver
{
    [SerializeField]
    private KitchenGameHandler _gameHandler;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private GameObject _cameraNode;

    [SerializeField]
    private GameObject _scoreUI;

    [SerializeField]
    private float _tweenTime = 1.0f;

    [SerializeField]
    private string _nextScene;

    [SerializeField]
    private GameObject _completedDishNode;

    private bool _gamefinished = false;
    public GameObject blackOutSquare;

    // Start is called before the first frame update
    private void Start()
    {
        Subscribe(_gameHandler);
    }

    // Update is called once per frame
    private void Update()
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
        var dish = Instantiate(_gameHandler.GetChosenDish().transform.parent.gameObject);
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

    private IEnumerator LoadScene()
    {
        yield return null;

        var objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;
        float fadeSpeed = 5;

        //fade to black
        while (blackOutSquare.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + fadeSpeed * Time.deltaTime;

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
            yield return null;
        }

        //Begin to load specified scene
        var asyncLoad = SceneManager.LoadSceneAsync(_nextScene);
    }
}