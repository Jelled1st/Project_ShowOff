using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreSceneController : MonoBehaviour
{
    [Serializable]
    public class GameScore
    {
        [SerializeField]
        private TextMeshProUGUI _score;

        [SerializeField]
        private TextMeshProUGUI _name;

        public void SetScore(string name, float score)
        {
            var splitName = name.ToList().Select(n => n.ToString().ToUpper()).Aggregate((t, next) => t + ' ' + next)
                .TrimEnd(' ');

            _name.SetText(splitName);
            _score.SetText(score.ToString());
        }
    }

    [SerializeField]
    private GameScore[] _scores;

    [SerializeField]
    private GameObject _notInHighscoresGameObject;

    [SerializeField]
    private GameScore _notInHighscoresScore;

    [SerializeField]
    private GameObject _keyboard;

    [SerializeField]
    private GameObject _scoreParent;

    [SerializeField]
    private TextMeshProUGUI _inputText;

    [SerializeField]
    private int _maxLetters;

    private void OnEnable()
    {
        foreach (var userScore in Scores.GetScoreList())
        {
            print($"{userScore.id}:{userScore.username}:{userScore.score}");
        }

        _notInHighscoresGameObject = _notInHighscoresGameObject.NullIfEqualsNull();
        _notInHighscoresGameObject?.SetActive(false);

        _scoreParent.SetActive(false);
        _keyboard.SetActive(true);

        _inputText.SetText("");

        KeyboardScript.KeyPressed += AppendInput;
        KeyboardScript.BackspacePressed += SubtractInput;
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
    }

    private void SubtractInput()
    {
        if (_inputText.text.Length > 0)
        {
            _inputText.text = _inputText.text.Substring(0, _inputText.text.Length - 1);
        }
    }

    private void TryToAppendScore()
    {
        if (_inputText.text.Length == 3)
        {
            if (!BadWordsList.IsBadWord(_inputText.text))
            {
                var currentScore = Scores.AppendScoreToLeaderboard(_inputText.text);

                var scoreList = Scores.GetScoreList();
                scoreList.Sort();
                print(scoreList.Count);
                for (var i = 0; i < scoreList.Count; i++)
                {
                    _scores[i].SetScore(scoreList[i].username, scoreList[i].score);
                }

                if (!currentScore.Item1)
                {
                    _notInHighscoresGameObject?.SetActive(true);
                    _notInHighscoresScore?.SetScore(currentScore.Item2.username, currentScore.Item2.score);
                }

                _keyboard.SetActive(false);
                _scoreParent.SetActive(true);
            }
            else
            {
                //TODO: Tell can't append the word
            }
        }
        else
        {
            //TODO: Tell input should be 3 letters
        }
    }
}