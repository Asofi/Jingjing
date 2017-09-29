using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public CanvasGroup StartScreen;

    public Text BestScoreText;
    public Text CurMenuScoreText;
    public Text CurrentScoreText;
    public Text Stars;

    public CanvasGroup StartButtonObject;
    public CanvasGroup BuyButtonObject;

    public Slider Lives;

    public Transform Star;

    public CanvasGroup SecondChance;
    public CanvasGroup HideOnPauseGroup;
    public CanvasGroup SkinButtonsGroup;
    public CanvasGroup AchievementsGroup;
    public GameObject Restart;
    public SpriteRenderer Header;

	void Start () {
        EventManager.OnGameStart += EventManager_OnGameStart;
        EventManager.OnGameOver += EventManager_OnGameOver;
        EventManager.OnSecondChance += EventManager_OnSecondChance;
        EventManager.OnAddScore += EventManager_OnAddScore;
	}

    private void EventManager_OnAddScore(int amount) {
        CurMenuScoreText.text = "SCORE: " + SuperManager.Instance.ScoreManager.CurrentScore;
    }

    private void EventManager_OnSecondChance()
    {
        Star.localScale = Vector2.one/2;
        ShowCanvasGroup(true, 0, SecondChance);
    }

    private void EventManager_OnGameOver()
    {
        ShowCanvasGroup(true, 0, SkinButtonsGroup);
        ShowCanvasGroup(true, 0, AchievementsGroup);
        Star.localScale = Vector2.one/2;
        CurMenuScoreText.gameObject.SetActive(true);
        Header.enabled = true;
        //SkinButtonsGroup.interactable = true;
        ShowCanvasGroup(true, 0, StartScreen);
        Restart.SetActive(false);
    }

    private void EventManager_OnGameStart()
    {
        Star.localScale = Vector2.one/2;
        CurMenuScoreText.text = "SCORE: " + 0;
        ShowCanvasGroup(false, 0.2f, StartScreen);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && SuperManager.Instance.GM.IsStarted && !SuperManager.Instance.GM.onSecondChanceMenu)
            PauseButton();
    }

    public void HideElementsOnPause(bool _flag)
    {
        ShowCanvasGroup(_flag, 0, HideOnPauseGroup);
    }

    public void ChangeText(Text obj, string text)
    {
        obj.text = text;
    }

    /// <summary>
    /// Func for smooth showing and hiding canvas groups
    /// </summary>
    /// <
    /// <param name="dir">True - show, False - hide</param>
    /// <param name="time">Duration of fading</param>
    /// <returns></returns>
    IEnumerator ShowCG(CanvasGroup CG, bool dir, float time)
    {
        float startAlpha = dir ? 0 : 1;
        float targetAlpha = dir ? 1 : 0;
        CG.interactable = dir;
        CG.blocksRaycasts = dir;
        if (time == 0)
        {
            CG.alpha = targetAlpha;
            yield break;
        }
        else
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * 1 / time;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                CG.alpha = newAlpha;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void ShowCanvasGroup(bool flag = true, float time = 0, params CanvasGroup[] group)
    {
        foreach (CanvasGroup CG in group)
        {
            StartCoroutine(ShowCG(CG, flag, time));
        }
    }

    private bool isAllowedToPlay = true;
    public void SkinChange(bool _flag)
    {
        if (_flag == isAllowedToPlay)
            return;
        isAllowedToPlay = _flag;
        if (_flag)
        {
            ShowCanvasGroup(true, 0.4f, StartButtonObject);
            ShowCanvasGroup(false, 0.25f, BuyButtonObject);
        }
        else
        {
            ShowCanvasGroup(false, 0.25f, StartButtonObject);
            ShowCanvasGroup(true, 0.4f, BuyButtonObject);
        }

    }

    void Pause(bool _flag)
    {
        SuperManager.Instance.GM.Pause(_flag);
        ShowCanvasGroup(_flag, 0.1f, StartScreen);
        HideElementsOnPause(!_flag);

        if(_flag)
        {
            //SkinButtonsGroup.interactable = false;
            Restart.SetActive(true);
        }
    }

    void PlayTapSound() {
        AudioManager.PlayAudio("Tap");
    }

    #region Buttons

    public void GameOverButton()
    {
        SuperManager.Instance.GM.onSecondChanceMenu = false;
        ShowCanvasGroup(false, 0, SecondChance);
        EventManager.GameOver();
        PlayTapSound();
    }

    public void SecondChanceButton()
    {
        SuperManager.Instance.GM.SecondChanceApplied();
        ShowCanvasGroup(false, 0, SecondChance);
        Time.timeScale = 1;
        PlayTapSound();
    }

    public void StartButton()
    {
        Time.timeScale = 1;
        Header.enabled = false;
        if (!SuperManager.Instance.GM.IsStarted)
            EventManager.GameStart();
        else
        {
            Pause(false);
        }

        PlayTapSound();
    }

    public void RestartButton() {
        EventManager.GameOver();
        EventManager.GameStart();
        Header.enabled = false;
        Time.timeScale = 1;
        PlayTapSound();
    }

    public void PauseButton()
    {
        Header.enabled = true;
        ShowCanvasGroup(true, 0.1f, StartScreen);
        ShowCanvasGroup(false, 0, SkinButtonsGroup);
        ShowCanvasGroup(false, 0, AchievementsGroup);
        Pause(true);
        PlayTapSound();
    }

    public void LeaderboardButton()
    {
        if (Social.localUser.authenticated) {
#if UNITY_ANDROID
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).ShowLeaderboardUI(SuperManager.Instance.ScoreManager.AndroidLeaderboardID);
#else
        UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowLeaderboardUI(SuperManager.Instance.ScoreManager.IOSLeaderboardID, TimeScope.AllTime);   
#endif
        } else
            SuperManager.Instance.ScoreManager.Auth();

        //Social.ShowLeaderboardUI();

        PlayTapSound();
    }

    public void AchieviementsButton()
    {
        if (Social.localUser.authenticated) {
            Social.ShowAchievementsUI();
        } else {
            SuperManager.Instance.ScoreManager.Auth();
        }
        PlayTapSound();
    }


    #endregion
}
