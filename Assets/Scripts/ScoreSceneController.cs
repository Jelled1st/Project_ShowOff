using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSceneController : MonoBehaviour
{
    [Serializable]
    public class GameScore
    {
        [SerializeField]
        private TextMeshProUGUI _score;

        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private RawImage _dishRender;

        public void SetScore(string name, float score, Texture dish)
        {
            var splitName = name.ToList().Select(n => n.ToString().ToUpper()).Aggregate((t, next) => t + ' ' + next)
                .TrimEnd(' ');

            _name.SetText(splitName);
            _score.SetText(score.ToString());
            _dishRender.texture = dish;
        }
    }

    [Serializable]
    public class RenderTextures
    {
        [SerializeField]
        public Texture BurgerAndFries;

        [SerializeField]
        public Texture ChiliCheeseFries;

        [SerializeField]
        public Texture FishAndChip;
    }

    [SerializeField]
    private RenderTextures _dishRenderTextures;

    [SerializeField]
    private GameScore[] _scores;

    [SerializeField]
    private RawImage _currentUserRenderTexture;

    [SerializeField]
    private KeyboardScript _keyboard;

    [SerializeField]
    private GameObject _scoreParent;

    [SerializeField]
    private GameObject _inputVisuals;

    [SerializeField]
    private TextMeshProUGUI _inputText;

    [SerializeField]
    private int _maxLetters;

    public static bool ShowInput = true;

    private void OnEnable()
    {
        if (ShowInput)
        {
            _scoreParent.SetActive(false);
            _keyboard.enabled = true;
            _keyboard.gameObject.SetActive(true);
            _keyboard.InputEnabled = true;

            _inputVisuals.SetActive(true);
            _currentUserRenderTexture.texture = GetDishTexture(Scores.GetCurrentUser().dish);
            _inputText.SetText("");

            KeyboardScript.KeyPressed += AppendInput;
            KeyboardScript.BackspacePressed += SubtractInput;
        }
        else
        {
            _keyboard.enabled = false;
            _keyboard.gameObject.SetActive(false);
            _inputVisuals.SetActive(false);
            ShowScores();
            ShowInput = true;
        }
    }

    private void OnDisable()
    {
        KeyboardScript.KeyPressed -= AppendInput;
        KeyboardScript.BackspacePressed -= SubtractInput;
    }

    private void AppendInput(char letter)
    {
        if (letter == KeyboardScript.EnterChar)
        {
            TryToAppendScore();
            return;
        }

        if (_inputText.text.Length < _maxLetters)
        {
            _inputText.text += letter;
        }

        if (BadWordsList.IsBadWord(_inputText.text))
        {
            var textInitialColor = _inputText.color;
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    _keyboard.InputEnabled = false;
                    _inputText.text = "@#%";
                    _inputText.color = Color.red;
                })
                .Join(_inputText.transform.DOShakePosition(.5f, 50f))
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    _keyboard.InputEnabled = true;
                    _inputText.text = "";
                    _inputText.color = textInitialColor;
                });
        }
    }

    private void SubtractInput()
    {
        if (_inputText.text.Length > 0)
        {
            _inputText.text = _inputText.text.Substring(0, _inputText.text.Length - 1);
        }
    }

    private Texture GetDishTexture(Dish.DishTypes dishType)
    {
        switch (dishType)
        {
            case Dish.DishTypes.BurgerAndFries:
                return _dishRenderTextures.BurgerAndFries;
            case Dish.DishTypes.ChiliCheeseFries:
                return _dishRenderTextures.ChiliCheeseFries;
            case Dish.DishTypes.FishAndChips:
                return _dishRenderTextures.FishAndChip;
            default:
                Debug.LogError($"Can't process dish of type {dishType.ToString()}");
                return _dishRenderTextures.FishAndChip;
                return null;
        }
    }

    private void ShowScores()
    {
        var scoreList = Scores.GetScoreList();
        scoreList.Sort();

        for (var i = 0; i < scoreList.Count; i++)
        {
            _scores[i].SetScore(scoreList[i].username, scoreList[i].score, GetDishTexture(scoreList[i].dish));
        }

        _scoreParent.SetActive(true);
    }

    private void TryToAppendScore()
    {
        if (_inputText.text.Length == 3)
        {
            Scores.AppendScoreToLeaderboard(_inputText.text);

            ShowScores();

            _inputVisuals.SetActive(false);
            _keyboard.enabled = false;
        }
        else
        {
            var charactersInput = "";

            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    _keyboard.InputEnabled = false;
                    for (int i = _inputText.text.Length; i < _maxLetters; i++)
                    {
                        charactersInput += "<color=red>X</color>";
                    }

                    _inputText.text += charactersInput;
                })
                .Join(_inputText.transform.DOShakePosition(0.5f, 50f))
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    _inputText.text = _inputText.text.Replace(charactersInput, "");
                    _keyboard.InputEnabled = true;
                });
        }
    }
}