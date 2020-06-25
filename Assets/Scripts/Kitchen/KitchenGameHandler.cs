using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KitchenGameHandler : MonoBehaviour, ISubject, IDishObserver
{
    [SerializeField]
    private List<Dish> _dishes;

    [SerializeField]
    private OnEventPlayer _onEventPlayer;

    [SerializeField]
    private KitchenTutorial _kitchenTutorial;

    private Dish _choosenDish;

    private List<IObserver> _observers = new List<IObserver>();
    private bool _paused = false;
    private bool _pausedForTutorial = false;
    private bool _gameFinished = false;

    private List<IIngredient> _ingredients;
    private List<GameObject> _ingredientGOs;

    [SerializeField]
    private bool _disableIngredients = true;

    public GameObject blackOutSquare;
    public GameObject blackoutCanvas;

    private void Awake()
    {
        gameObject.tag = "GameHandler";
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (blackoutCanvas.activeInHierarchy == false)
        {
            blackoutCanvas.SetActive(true);
        }

        StartCoroutine(FadeIn());

        _ingredients = new List<IIngredient>();
        _ingredientGOs = new List<GameObject>();
        var ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        for (var i = 0; i < ingredients.Length; ++i)
        {
            _ingredients.Add(ingredients[i].GetComponent<IIngredient>());
            _ingredientGOs.Add(ingredients[i]);
            if (_disableIngredients) ingredients[i].SetActive(false);
        }

        _kitchenTutorial.gameHandler = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals) && _choosenDish == null) ChooseDish(_dishes[0]);
        if (_pausedForTutorial && Input.GetMouseButtonDown(0))
        {
            UnPause();
        }
    }

    public void SubscribeToAllIngredients(IObserver observer)
    {
        for (var i = 0; i < _ingredients.Count; ++i)
        {
            _ingredients[i].Register(observer);
        }
    }

    public void ChooseDish(Dish dish)
    {
        for (var i = 0; i < _dishes.Count; ++i)
        {
            if (_dishes[i] == dish)
            {
                _choosenDish = _dishes[i];
                EnableDishIngredients(_choosenDish);
                Subscribe(_choosenDish);
                var sideDishes = _choosenDish.GetSideDishesLeft();
                for (var j = 0; j < sideDishes.Count; ++j)
                {
                    Subscribe(sideDishes[j]);
                }

                _onEventPlayer.Subscribe(_choosenDish);
                _kitchenTutorial.ChooseDish(dish);
                Scores.SetCurrentDish(dish.GetDishType());
            }
            else
            {
                Destroy(_dishes[i].transform.parent.gameObject);
            }
        }
    }

    public Dish GetChosenDish()
    {
        return _choosenDish;
    }

    public IEnumerator FadeIn(bool fadeToWhite = true, int fadeSpeed = 5)
    {
        var objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToWhite)
        {
            while (blackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - fadeSpeed * Time.deltaTime;

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }

    private void EnableDishIngredients(Dish dish)
    {
        var ingredientTypes = new List<IngredientType>(dish.GetAllPossibleIngredients());

        for (var i = _ingredients.Count - 1; i >= 0; --i)
        {
            if (ingredientTypes.Contains(_ingredients[i].GetIngredientType()))
            {
                _ingredientGOs[i].SetActive(true);
                if (_ingredients[i] is ISubject) _onEventPlayer.Subscribe(_ingredients[i] as ISubject);
            }
            else
            {
                Destroy(_ingredientGOs[i]);
                _ingredientGOs.RemoveAt(i);
                _ingredients.RemoveAt(i);
            }
        }
    }

    public void Pause(bool tutorialPause)
    {
        _pausedForTutorial = tutorialPause;
        if (!_paused)
        {
            _paused = true;
            for (var i = 0; i < _observers.Count; ++i)
            {
                if (_observers is IGameHandlerObserver) (_observers[i] as IGameHandlerObserver).OnPause();
            }
        }
    }

    public void UnPause()
    {
        if (_paused)
        {
            _pausedForTutorial = false;
            _paused = false;
            for (var i = 0; i < _observers.Count; ++i)
            {
                if (_observers is IGameHandlerObserver) (_observers[i] as IGameHandlerObserver).OnContinue();
            }
        }
    }

    public void FinishGame()
    {
        if (!_gameFinished)
        {
            for (var i = 0; i < _observers.Count; ++i)
            {
                if (_observers[i] is IGameHandlerObserver)
                {
                    (_observers[i] as IGameHandlerObserver).OnFinish();
                }
            }

            _gameFinished = true;
        }
    }

    public void Register(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnRegister(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
    }

    #region IDishObserver

    public void OnIngredientAdd(ISubject subject, IIngredient ingredient)
    {
        Scores.AddScore(250);
    }

    public void OnFinishDish(ISubject subject)
    {
        if (subject as Dish == _choosenDish) FinishGame();
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    #endregion
}