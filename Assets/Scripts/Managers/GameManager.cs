using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    const string IS_FIRST_START = "is_first_start";
    [Header("Ball Spawn Settings")]
    public AnimationCurve BallStartingMove;
    public Vector2 StartPoint;
    public Vector2 PausePoint;
    public float LowerAlpha;
    public float UpperAlpha;
    public float StartSpeed;
    public Transform BallPrefab;
    private Transform ball;
    [Space]
    [Header("Game Settings")]
    public int StartLives;
    int currentLives;
    [Space]
    [Header("Shop Settings")]
    public int Stars = 0;

    private bool isStarted = false;
    public bool isSecondChance = false;
    [HideInInspector]
    public bool onSecondChanceMenu = false;
    public bool IsFirstStart = true;

    public int CurrentLives
    {
        get
        {
            return currentLives;
        }

        set
        {
            currentLives = value;
            SuperManager.Instance.GUIManager.Lives.value = Mathf.Clamp(currentLives, 0, 3);
        }
    }

    public bool IsStarted
    {
        get
        {
            return isStarted;
        }

        set
        {
            if (isStarted == value)
                return;
            SuperManager.Instance.GUIManager.HideElementsOnPause(value);
            isStarted = value;
        }
    }

    private void Awake() {
        if (!PlayerPrefs.HasKey(IS_FIRST_START)) {
            PlayerPrefsX.SetBool(IS_FIRST_START, false);
        } else
            IsFirstStart = false;
    }

    // Use this for initialization
    void Start () {
        EventManager.OnGameStart += EventManager_OnGameStart;
        EventManager.OnGameOver += EventManager_OnGameOver;
        EventManager.OnSecondChance += EventManager_OnSecondChance;
        EventManager.OnAddScore += EventManager_OnAddScore;
        CurrentLives = StartLives;

        AudioManager.PlayAudioControllable("Soundtrack");

        Application.targetFrameRate = 60;
	}

    private void EventManager_OnAddScore(int amount)
    {
        int _curScore = SuperManager.Instance.ScoreManager.currentScore;
        if (_curScore % 100 == 0 && _curScore != 0 && currentLives < 3)
        {
            CurrentLives++;
            print("LIFE ADDED");
        }
    }

    private void EventManager_OnSecondChance()
    {
        isSecondChance = true;
        onSecondChanceMenu = true;
        EZ_Pooling.EZ_PoolManager.Despawn(ball);
        Time.timeScale = 0;
        //StartCoroutine(SpawnBall());
    }

    public void SecondChanceApplied()
    {
        CurrentLives++;
        StartCoroutine(SpawnBall());
    }

    private void Update()
    {
        if(ball != null && ball.position.y <= -8)
        {
            EZ_Pooling.EZ_PoolManager.Despawn(ball);
            ball = null;
            if (CurrentLives <= 1)
            {
                if (!isSecondChance) {
                    EventManager.SecondChance();
                    print("SecChance");
                } else {
                    //EventManager.GameOver();
                    StartCoroutine(GameOver());
                    print("GameOver");
                }
            }
            //EventManager.GameOver();
            else
            {
                StartCoroutine(SpawnBall());
            }
            CurrentLives--;
        }
    }

    IEnumerator GameOver() {
        yield return SuperManager.Instance.Spawner.MoveBricksUp();
        EventManager.GameOver();
    }

    private void EventManager_OnGameOver()
    {
        IsStarted = false;
        isSecondChance = false;
        EZ_Pooling.EZ_PoolManager.Despawn(ball);
        ball = null;     
        StopAllCoroutines();
    }

    private void EventManager_OnGameStart()
    {
        StartCoroutine(SpawnBall());
        IsStarted = true;
        CurrentLives = StartLives;
    }

    public void Pause(bool _flag)
    {
        if (pauseTime != null)
            StopCoroutine(pauseTime);
        StartCoroutine(pauseTime = PauseTime(_flag));
    }

    IEnumerator pauseTime;
    IEnumerator PauseTime(bool _flag)
    {
        float _startTimeSpeed = Time.timeScale;
        float _endTimeSpeed = _flag? 0 : 1;
        float t = 0;
        while(t <= 1)
        {
            t += Time.unscaledDeltaTime * 10;
            float _newTimeSpeed = Mathf.Lerp(_startTimeSpeed, _endTimeSpeed, t);
            Time.timeScale = _newTimeSpeed;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
    }

    IEnumerator SpawnBall()
    {
        //SuperManager.Instance.Spawner.curBricksSpeed /= 3;
        //if (CurrentLives == -1)
        //    CurrentLives = 0;
        ball = EZ_Pooling.EZ_PoolManager.Spawn(BallPrefab, StartPoint, Quaternion.identity);
        ball.localScale = Vector2.one * 0.8f;
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        float t = 0;
        float y = 0;
        while (t <= 1)
        {
            Vector2 newPos = Vector2.Lerp(StartPoint, PausePoint, y);
            t += Time.deltaTime * 5;
            y = BallStartingMove.Evaluate(t);
            ball.position = newPos;
            //print(string.Format("t = {0}; y = {1}",t,y));
            yield return new WaitForEndOfFrame();
        }
        t = 0;

        while (t <= 1)
        {
            float speed = 6000 * t;
            ball.Rotate(Vector3.forward * speed * Time.deltaTime);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        float alpha = Random.Range(LowerAlpha, UpperAlpha);
        Vector2 dir = new Vector2(Mathf.Sin(alpha), Mathf.Cos(alpha)) - (Vector2)ball.transform.position;
        rb.simulated = true;
        rb.AddForce(dir * StartSpeed);
        //SuperManager.Instance.Spawner.curBricksSpeed *= 3;
    }
}
