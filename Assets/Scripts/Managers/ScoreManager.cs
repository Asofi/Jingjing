using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public class ScoreManager : MonoBehaviour {

    public const string BEST_SCORE = "best_score";
    public const string ALL_SCORE = "all_score";
    public string AndroidLeaderboardID = "CgkIqNyn1rkEEAIQAA";
    public string IOSLeaderboardID = "CgkIqNyn1rkEEAIQAA";
    private string boardID;
    private int bestScore;
    public int currentScore;
    public int allScore;

    public int BestScore
    {
        get
        {
            return bestScore;
        }

        set
        {
            bestScore = value;
            SuperManager.Instance.GUIManager.ChangeText(SuperManager.Instance.GUIManager.BestScoreText, "BEST: " + bestScore.ToString());
        }
    }

    public int CurrentScore
    {
        get
        {
            return currentScore;
        }

        set
        {
            currentScore = value;
            SuperManager.Instance.GUIManager.ChangeText(SuperManager.Instance.GUIManager.CurrentScoreText, currentScore.ToString());
        }
    }

    private void Awake()
    {
        BestScore = PlayerPrefs.GetInt(BEST_SCORE, 0);
        allScore = PlayerPrefs.GetInt(ALL_SCORE, 0);
        CurrentScore = 0;
    }
    // Use this for initialization
    void Start () {
        EventManager.OnGameStart += EventManager_OnGameStart;
        EventManager.OnGameOver += EventManager_OnGameOver;
        EventManager.OnAddScore += EventManager_OnAddScore;

#if UNITY_ANDROID
        boardID = AndroidLeaderboardID;
        PlayGamesPlatform.Activate();
#else
        boardID = IOSLeaderboardID;
#endif
        Auth();

    }

    public void Auth() {
        Social.localUser.Authenticate((bool success) => {
            if (success) {
                Social.ReportScore(BestScore, boardID, (bool success1) => {
                    // handle success or failure
                });
                if (SuperManager.Instance.GM.IsFirstStart) {
                    AchiviementsManager.Instance.IncrementingAchivie("Skin Collector");
                }
            }
            else
                Debug.LogError("Can't login to Google Games");
        });
    }

    private void EventManager_OnAddScore(int amount)
    {
        CurrentScore += amount;
        allScore += amount;
        CheckForScoreAchiviements(CurrentScore);
    }

    void CheckForScoreAchiviements(int score) {
        int _rookie = 200;
        int _amateur = 400;
        int _pro = 600;
        int _gamer = 800;

        if (CurrentScore >= _gamer)
            AchiviementsManager.Instance.UnlockAchivie("Gamer");
        else if (CurrentScore >= _pro)
            AchiviementsManager.Instance.UnlockAchivie("Pro");
        else if (CurrentScore >= _amateur)
            AchiviementsManager.Instance.UnlockAchivie("Amateur");
        else if (CurrentScore >= _rookie)
            AchiviementsManager.Instance.UnlockAchivie("Rookie");

        if (!SuperManager.Instance.GM.isSecondChance) {
            if (CurrentScore >= _gamer)
                AchiviementsManager.Instance.UnlockAchivie("Super-Gamer");
            else if (CurrentScore >= _pro)
                AchiviementsManager.Instance.UnlockAchivie("Super-Pro");
            else if (CurrentScore >= _amateur)
                AchiviementsManager.Instance.UnlockAchivie("Super-Amateur");
            else if (CurrentScore >= _rookie)
                AchiviementsManager.Instance.UnlockAchivie("Super-Rookie");
        }
    }

    private void EventManager_OnGameOver()
    {
        if (CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
            PlayerPrefs.SetInt(BEST_SCORE, BestScore);
            Social.ReportScore(BestScore, "CgkIqNyn1rkEEAIQAA", (bool success) => {
                // handle success or failure
            });
        }
    }

    private void EventManager_OnGameStart()
    {
        CurrentScore = 0;
    }

}
