﻿using UnityEngine;

public class ScoreDisplay : LanguageText, IScoresObserver
{
    [SerializeField]
    private float _scoreAddSpeed = 2.0f;

    private float _displayScore = 0;
    private float _unAddedScore;
    private string _preText = "Score: ";

    private void Start()
    {
        Subscribe();
        if (_fieldName == "") _fieldName = "misc.score";
        if (LanguageHandler.instance != null) _preText = LanguageHandler.instance.Register(this, _fieldName) + ": ";
        _textMeshProUgui.text = _preText + _displayScore + "!";
    }

    private void Update()
    {
        if (_unAddedScore != 0.0f)
        {
            var amount = 0.0f;
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
            _textMeshProUgui.text = _preText + _displayScore + "!";
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

    #region LanguageText

    public override void SetText(string text)
    {
        _preText = text + ": ";
    }

    public override string GetFieldName()
    {
        return _fieldName;
    }

    #endregion
}