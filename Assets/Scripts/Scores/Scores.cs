using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Scores
{
    public const int MachineCompleteBreakage = -500;
    public const int MachineStageTwoBreakage = 150;
    public const int LeftTimeMultiplier = 100;

    public const int WasherPassed = 100;
    public const int PeelerPassed = 100;
    public const int CutterPassed = 150;
    public const int PackerPassed = 150;

    public const int MachineFixed = 200;

    public const int LoadedPotatoBagged = 100;

    private static List<IScoresObserver> _observers = new List<IScoresObserver>();

    public class UserScore : IComparable
    {
        public int id;
        public string username;
        public float score;

        public UserScore(string username, float score, int id)
        {
            this.username = username;
            this.score = score;
            this.id = id;
        }

        public int CompareTo(object obj)
        {
            var other = (UserScore) obj;
            return other.score.CompareTo(this.score);
        }
    }

    private const string ScorePrefix = "Score";
    private const string NamePrefix = "Name";
    private const string BlankName = "NNM";
    private const int MaxScoresCount = 10;

    private static UserScore _currentUser;

    private static readonly List<UserScore> ScoreList = new List<UserScore>();

    public static List<UserScore> GetScoreList()
    {
        return new List<UserScore>(ScoreList);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        RefreshScores();
    }

    private static void RefreshScores()
    {
        ScoreList.Clear();

        var i = 0;
        while (PlayerPrefs.HasKey(ScorePrefix + i))
        {
            ScoreList.Add(
                new UserScore(PlayerPrefs.GetString(NamePrefix + i), PlayerPrefs.GetFloat(ScorePrefix + i), i));
            i++;
        }

        ScoreList.Sort();
        ScoreList.Reverse();
    }

    public static void ClearScores()
    {
        for (int i = 0; i < MaxScoresCount; i++)
        {
            PlayerPrefs.DeleteKey(NamePrefix + i);
            PlayerPrefs.DeleteKey(ScorePrefix + i);
            ScoreList.Clear();
        }
    }

    public static float GetCurrentScore()
    {
        return _currentUser.score;
    }

    public static void AddScore(float score)
    {
        if (_currentUser == null)
            _currentUser = new UserScore(BlankName, 0f, -1);

        _currentUser.score += score;
        NotifyAddScore(score);
        //Debug.Log(_currentUser.score);
    }

    public static void SubScore(float score)
    {
        if (_currentUser == null)
            _currentUser = new UserScore(BlankName, 0f, -1);

        _currentUser.score -= score;
        NotifySubScore(score);
        // Debug.Log(_currentUser.score);
    }

    public static void AdjustScore(float score)
    {
        if (_currentUser == null)
            _currentUser = new UserScore(BlankName, 0f, -1);

        _currentUser.score += score;
        NotifyAdjustScore(score);
        //Debug.Log(_currentUser.score);
    }

    public static void AppendScoreToLeaderboard(string username)
    {
        RefreshScores();

        var newId = 0;

        if (ScoreList.Count > 0)
        {
            newId = ScoreList.Count;
        }
        else if (ScoreList.Count > MaxScoresCount)
        {
            var minScore = ScoreList.Min(s => s.score);
            if (_currentUser.score > minScore)
            {
                newId = ScoreList.Find(s => s.score == minScore).id;
            }
            else
            {
                _currentUser = null;
                return;
            }
        }

        _currentUser.username = username;

        PlayerPrefs.SetString(NamePrefix + newId, _currentUser.username);
        PlayerPrefs.SetFloat(ScorePrefix + newId, _currentUser.score);

        _currentUser = null;
        RefreshScores();
    }

    public static void Register(IScoresObserver observer)
    {
        _observers.Add(observer);
    }

    public static void UnRegister(IScoresObserver observer)
    {
        _observers.Remove(observer);
    }

    private static void NotifyAddScore(float score)
    {
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].AddedScore(score);
        }
    }

    private static void NotifySubScore(float score)
    {
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].SubtractedScore(score);
        }
    }

    private static void NotifyAdjustScore(float score)
    {
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].AdjustedScore(score);
        }
    }
}