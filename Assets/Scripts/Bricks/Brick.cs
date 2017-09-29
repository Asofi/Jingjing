using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

    public static int ___killedBricks = 0;
    public static float ___time;
    public static bool ___isRecording = false;

    protected Animator mAnimator;
    private Collider2D mCollider;
    protected BrickCounter BC;

    public Sprite[] SpriteVariations;
    public int HitsToDestroy = 1;
    public int ScoreCost = 1;

    protected Transform BrickGraphics;
    public TextMesh Text;

    private bool isAlive = false;
    protected int hitsLeft;
    //private SpriteRenderer spriteRenderer;

    protected virtual void Awake () {
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mAnimator = GetComponent<Animator>();
        mAnimator.SetFloat("offset", Random.Range(0f, 1f));
        mCollider = GetComponent<Collider2D>();
        BrickGraphics = transform.Find("Graphics");
        BC = BrickGraphics.GetComponent<BrickCounter>();
        //if (!___isRecording) {
        //    SuperManager.Instance.StartCoroutine(___KillCounter());
        //    ___isRecording = true;
        //}
    }

    static IEnumerator ___KillCounter() {
        while (true) {
            ___time = Time.timeSinceLevelLoad / 60;
            print(string.Format("You've killed {0} bricks for {1} minutes!", ___killedBricks, ___time));
            yield return new WaitForSeconds(6);
        }
    }

    private void Start()
    {
        EventManager.OnCandycallipce += EventManager_OnCandycallipce;
    }

    private void EventManager_OnCandycallipce() {
        EZ_Pooling.EZ_PoolManager.Despawn(transform);
    }

    private void Update()
    {
        if (transform.position.y < -2.5 && isAlive)
            if (!SuperManager.Instance.GM.isSecondChance) {
                EventManager.SecondChance();
            }
            else {
                EventManager.GameOver();
            }
    }

    int randomValue = 1;
    protected virtual void OnSpawned()
    {
        randomValue = Random.Range(0, SpriteVariations.Length);
        EventManager.OnGameOver += EventManager_OnGameOver;
        isAlive = true;
        mCollider.enabled =  true;
        hitsLeft = HitsToDestroy;
        BrickCounter.BrickCount++;
        //print(BrickCounter.BrickCount);
        BC.isVisible = true;

        float _randomIdleStart = Random.Range(0, mAnimator.GetCurrentAnimatorStateInfo(0).length); //Set a random part of the animation to start from
        mAnimator.Play("Idle" + (randomValue + 1).ToString(), -1, _randomIdleStart);
    }

    void OnDespawned()
    {

    }

    protected IEnumerator DelayedDespawn(float time)
    {
        mAnimator.Play("Death" + (randomValue + 1).ToString(),-1, 0);
        mCollider.enabled = false;
        isAlive = false;
        EventManager.OnGameOver -= EventManager_OnGameOver;
        yield return new WaitForSeconds(time);
        //print("Despawn " + transform.name);
        EZ_Pooling.EZ_PoolManager.Despawn(transform);
    }

    private void EventManager_OnGameOver()
    {
        //print("Despawn on GO " + transform.name);
        StopAllCoroutines();
        if(transform != null)
            EZ_Pooling.EZ_PoolManager.Despawn(transform);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        hitsLeft--;
        if (hitsLeft <= 0)
        {
            ___killedBricks++;
            if (mAnimator != null)
                StartCoroutine(DelayedDespawn(1.5f));
            else {
                //print("Despawn on collision " + transform.name);
                EZ_Pooling.EZ_PoolManager.Despawn(transform);
            }
            EventManager.AddScore(ScoreCost);
            BrickCounter.BrickCount--;
            //print(BrickCounter.BrickCount);
            BC.isVisible = false;
        }
    }

}
