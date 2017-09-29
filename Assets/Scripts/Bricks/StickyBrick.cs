using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBrick : Brick
{
    public float ShakeAmount = 0.2f;
    private Ball ball;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.PlayAudio(ShopManager.Instance.CurrentSkin.StickySound);
        EventManager.AddScore(1);
        BrickCounter.BrickCount--;
        //print(BrickCounter.BrickCount);
        BC.isVisible = false;
        if (ball != null)
            return;
        ball = collision.GetComponent<Ball>();
        StartCoroutine(EatBall(0.6f));

    }

    IEnumerator EatBall(float time)
    {
        float t = time;
        Rigidbody2D ballRB = ball.rb;
        ballRB.simulated = false;
        Transform ballTransform = ball.transform;
        Vector2 startBallSize = Vector2.one * 0.8f;
        Vector2 targetBallSize = Vector2.zero;
        while (t > 0)
        {
            ball.transform.position = Vector2.MoveTowards(ball.transform.position, transform.position, ballRB.velocity.magnitude/2 * Time.deltaTime);
            if(t > time - 1)
                ballTransform.localScale = Vector2.Lerp(targetBallSize, startBallSize, t - (time - 1));
            if (t < 1)
                ballTransform.localScale = Vector2.Lerp(startBallSize, targetBallSize, t);
            t -= Time.deltaTime; 
            BrickGraphics.localPosition = new Vector2(Random.Range(0 - ShakeAmount, 0 + ShakeAmount),
                                                       Random.Range(0 - ShakeAmount, 0 + ShakeAmount));

            yield return new WaitForEndOfFrame();
        }
        BrickGraphics.localPosition = Vector2.zero;

        float alpha = Random.Range(-180, 180);
        Vector2 dir = new Vector2(Mathf.Sin(alpha), Mathf.Cos(alpha)) - (Vector2)ball.transform.position;
        //t = 1;
        //while (t > 0)
        //{
        //    ballTransform.localScale = Vector2.Lerp(targetBallSize, startBallSize, t);
        //    t -= Time.deltaTime /5;

        //    yield return new WaitForEndOfFrame();
        //}
        ballRB.simulated = true;
        ballRB.velocity = Vector2.zero;
        ballRB.AddForce(dir * 300);

        StartCoroutine(DelayedDespawn(1));
        ball = null;
        ballTransform.localScale = startBallSize;
        //EZ_Pooling.EZ_PoolManager.Despawn(transform);
    }

    protected override void OnSpawned()
    {
        //SpriteVariations = SuperManager.Instance.ShopManager.CurrentSkin.StickyBricks;
        mAnimator.runtimeAnimatorController = SuperManager.Instance.ShopManager.CurrentSkin.StickyController;
        base.OnSpawned();
    }
}
