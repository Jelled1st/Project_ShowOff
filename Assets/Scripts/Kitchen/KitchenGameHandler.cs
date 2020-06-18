using System.Collections.Generic;
using UnityEngine;

public class KitchenGameHandler : MonoBehaviour, ISubject, IDishObserver
{
    [SerializeField] List<Dish> _dishes;
    [SerializeField] OnEventPlayer _onEventPlayer;
    [SerializeField] KitchenTutorial _kitchenTutorial;
    [SerializeField] GameObject _polaroidScore;
    private Dish _choosenDish;

    private List<IObserver> _observers = new List<IObserver>();
    bool _paused = false;
    bool _pausedForTutorial = false;
    bool _gameFinished = false;

    private List<IIngredient> _ingredients;
    private List<GameObject> _ingredientGOs;

    [SerializeField] private bool _disableIngredients = true;

    void Awake()
    {
        this.gameObject.tag = "GameHandler";
        _polaroidScore.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _ingredients = new List<IIngredient>();
        _ingredientGOs = new List<GameObject>();
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        for(int i = 0; i < ingredients.Length; ++i)
        {
            _ingredients.Add(ingredients[i].GetComponent<IIngredient>());
            _ingredientGOs.Add(ingredients[i]);
            if(_disableIngredients) ingredients[i].SetActive(false);
        }
        _kitchenTutorial.gameHandler = this;

        //ChooseDish(_dishes[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals) && _choosenDish == null) ChooseDish(_dishes[0]);
        if (_pausedForTutorial && Input.GetMouseButtonDown(0))
        {
            UnPause();
        }
    }

    public void SubscribeToAllIngredients(IObserver observer)
    {
        for(int i = 0; i < _ingredients.Count; ++i)
        {
            _ingredients[i].Register(observer);
        }
    }

    public void ChooseDish(Dish dish)
    {
        for(int i = 0; i < _dishes.Count; ++i)
        {
            if (_dishes[i] == dish)
            {
                _choosenDish = _dishes[i];
                EnableDishIngredients(_choosenDish);
                Subscribe(_choosenDish);
                _onEventPlayer.Subscribe(_choosenDish);
                _kitchenTutorial.ChooseDish(dish);
            }
            else
            {
                Destroy(_dishes[i].transform.parent.gameObject);
            }
        }
    }

    private void EnableDishIngredients(Dish dish)
    {
        List<IngredientType> ingredientTypes = new List<IngredientType>(dish.GetAllPossibleIngredients());

        for(int i = _ingredients.Count-1; i >= 0; --i)
        {
            if(ingredientTypes.Contains(_ingredients[i].GetIngredientType()))
            {
                _ingredientGOs[i].SetActive(true);
                if(_ingredients[i] is ISubject) _onEventPlayer.Subscribe(_ingredients[i] as ISubject);
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
            for (int i = 0; i < _observers.Count; ++i)
            {
                if(_observers is IGameHandlerObserver) (_observers[i] as IGameHandlerObserver).OnPause();
            }
        }
    }

    public void UnPause()
    {
        if (_paused)
        {
            _pausedForTutorial = false;
            _paused = false;
            for (int i = 0; i < _observers.Count; ++i)
            {
                if (_observers is IGameHandlerObserver) (_observers[i] as IGameHandlerObserver).OnContinue();
            }
        }
    }

    public void FinishGame()
    {
        if (!_gameFinished)
        {
            _polaroidScore.SetActive(true);
            _gameFinished = true;
            for (int i = 0; i < _observers.Count; ++i)
            {
                if (_observers is IGameHandlerObserver) (_observers[i] as IGameHandlerObserver).OnFinish();
            }
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
        for(int i = 0; i < _observers.Count; ++i)
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
    }

    public void OnFinishDish(ISubject subject)
    {
        FinishGame();
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
