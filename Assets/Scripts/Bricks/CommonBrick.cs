using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonBrick : Brick
{

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);
        AudioManager.PlayAudio(ShopManager.Instance.CurrentSkin.CommonSound);
    }

    protected override void OnSpawned()
    {
        //SpriteVariations = SuperManager.Instance.ShopManager.CurrentSkin.CommonBricks;
        mAnimator.runtimeAnimatorController = SuperManager.Instance.ShopManager.CurrentSkin.CommonController;
        base.OnSpawned();
    }
}
