using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Scores
{
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

    public static void AddScore(float score)
    {
        if (_currentUser == null)
            _currentUser = new UserScore(BlankName, 0f, -1);

        _currentUser.score += score;
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
}