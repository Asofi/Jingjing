using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBrick : Brick
{
    public ParticleSystem breakPieces;

    protected override void OnSpawned()
    {
        //SpriteVariations = SuperManager.Instance.ShopManager.CurrentSkin.SolidBricks;

        mAnimator.runtimeAnimatorController = SuperManager.Instance.ShopManager.CurrentSkin.SolidController;
        base.OnSpawned();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (hitsLeft == 1)
        {
            breakPieces.Play();
            mAnimator.Play("Idle2");
            float _randomIdleStart = Random.Range(0, mAnimator.GetCurrentAnimatorStateInfo(0).length);
            mAnimator.Play("Idle2", -1, _randomIdleStart);
            AudioManager.PlayAudio(ShopManager.Instance.CurrentSkin.SolidSound);
        } else {
            AudioManager.PlayAudio(ShopManager.Instance.CurrentSkin.SolidCrashSound);
        }
    }
}
