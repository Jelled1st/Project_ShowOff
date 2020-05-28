using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour, IScoresObserver
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] float _scoreAddSpeed = 2.0f;
    [SerializeField] string preText = "Score: ";
    [SerializeField] string postText = "!";
    private float _displayScore;
    private float _unAddedScore;

    // Start is called before the first frame update
    void Start()
    {
        Subscribe();
    }

    // Update is called once per frame
    void Update()
    {
        if (_unAddedScore != 0.0f)
        {
            float amount = 0.0f;
            if (_unAddedScore < 0.0f)
            {
                amount = Mathf.Max(-_scoreAddSpeed, _unAddedScore);
            }
            else
            {
                amount = Mathf.Min(_scoreAddSpeed, _unAddedScore);
            }

            _displayScore += amount;
            _unAddedScore -= amount;
            _text.text = preText + _displayScore + postText;
        }
    }

    public void AddedScore(float score)
    {
        _unAddedScore += score;
    }

    public void AdjustedScore(float score)
    {
        _unAddedScore += score;
    }

    public void SubtractedScore(float score)
    {
        _unAddedScore -= score;
    }

    public void Subscribe()
    {
        Scores.Register(this);
    }

    public void UnSubscribe()
    {
        Scores.UnRegister(this);
    }
}
