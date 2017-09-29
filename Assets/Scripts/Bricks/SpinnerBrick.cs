using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerBrick : Brick
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        AudioManager.PlayAudio(ShopManager.Instance.CurrentSkin.SpinnerSound);

        Ball ball = collision.collider.GetComponent<Ball>();
        float alpha;
        if(transform.position.y > ball.transform.position.y)
        {
            alpha = Random.Range(-70, 70);
        }
        else
        {
            alpha = (Random.value > 0.5f) ? Random.Range(-160, -90) : Random.Range(90, 160);
        }
        Vector2 dir = new Vector2(Mathf.Sin(alpha), Mathf.Cos(alpha)) - (Vector2)ball.transform.position;
        Debug.DrawRay(ball.transform.position, dir, Color.red, 10);
        //ball.rb.velocity = Vector2.zero;
        ball.rb.velocity = dir.normalized * ball.rb.velocity.magnitude;
    }

    protected override void OnSpawned()
    {
        //SpriteVariations = SuperManager.Instance.ShopManager.CurrentSkin.SpinnerBricks;
        mAnimator.runtimeAnimatorController = SuperManager.Instance.ShopManager.CurrentSkin.SpinnerController;
        base.OnSpawned();
    }
}
