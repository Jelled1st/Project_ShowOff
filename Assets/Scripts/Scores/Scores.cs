using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dish.DishTypes dish;
        public float score;

        public UserScore(string username, float score, int id, Dish.DishTypes dish)
        {
            this.username = username;
            this.score = score;
            this.id = id;
            this.dish = dish;
        }

        public int CompareTo(object obj)
        {
            var other = (UserScore) obj;
            return other.score.CompareTo(score);
        }
    }

    private const string ScorePrefix = "Score";
    private const string NamePrefix = "Name";
    private const string DishPrefix = "Dish";
    private const string BlankName = "NNM";
    private const int MaxScoresCount = 3;

    private static UserScore _currentUser;

    private static readonly List<UserScore> ScoreList = new List<UserScore>();

    private static UserScore CurrentUser
    {
        get => _currentUser ?? (_currentUser = new UserScore(BlankName, 0f, -1, Dish.DishTypes.Undifined));
        set => _currentUser = value;
    }

    public static List<UserScore> GetScoreList()
    {
        RefreshScores();
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
            var username = PlayerPrefs.GetString(NamePrefix + i);
            var score = PlayerPrefs.GetFloat(ScorePrefix + i);

            if (!Enum.TryParse(PlayerPrefs.GetString(DishPrefix + i), out Dish.DishTypes dishType))
            {
                dishType = Dish.DishTypes.BurgerAndFries;
                Debug.LogWarning(
                    $"When loading the dish for user [{username}] the dish couldn't be found! Setting the default dish...");
            }

            ScoreList.Add(new UserScore(username, score, i, dishType));
            i++;
        }

        ScoreList.Sort();
        ScoreList.Reverse();
    }

    public static void ClearScores()
    {
        PlayerPrefs.DeleteAll();

        ScoreList.Clear();
    }

    public static UserScore GetCurrentUser()
    {
        return CurrentUser;
    }

    public static void SetCurrentDish(Dish.DishTypes dishType)
    {
        CurrentUser.dish = dishType;
    }

    public static void AddScore(float score)
    {
        CurrentUser.score += score;
        NotifyAddScore(score);
        //Debug.Log(_currentUser.score);
    }

    public static void SubScore(float score)
    {
        CurrentUser.score -= score;
        NotifySubScore(score);
        // Debug.Log(_currentUser.score);
    }

    public static void AdjustScore(float score)
    {
        CurrentUser.score += score;
        NotifyAdjustScore(score);
        //Debug.Log(_currentUser.score);
    }

    public static (bool, UserScore) AppendScoreToLeaderboard(string username)
    {
        RefreshScores();

        var newId = 0;

        var newUser = new UserScore(username, CurrentUser.score, -1, CurrentUser.dish);

        ScoreList.Add(newUser);
        ScoreList.Sort();

        var gotToTop = ScoreList.Take(MaxScoresCount).Contains(newUser);

        for (var i = 0; i < MaxScoresCount; i++)
        {
            if (ScoreList.Count <= i)
                break;

            PlayerPrefs.SetString(NamePrefix + i, ScoreList[i].username);
            PlayerPrefs.SetFloat(ScorePrefix + i, ScoreList[i].score);
            PlayerPrefs.SetString(DishPrefix + i, ScoreList[i].dish.ToString());
        }

        CurrentUser = null;
        return (gotToTop, newUser);
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
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].AddedScore(score);
        }
    }

    private static void NotifySubScore(float score)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].SubtractedScore(score);
        }
    }

    private static void NotifyAdjustScore(float score)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].AdjustedScore(score);
        }
    }
}