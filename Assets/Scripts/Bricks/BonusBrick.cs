using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBrick : Brick {
    public Transform StartPrefab;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        EventManager.AddCoin();
        AudioManager.PlayAudio(ShopManager.Instance.CurrentSkin.BonusSound);
        EZ_Pooling.EZ_PoolManager.Spawn(StartPrefab, transform.position, Quaternion.identity);
    }
    protected override void OnSpawned()
    {
        //SpriteVariations[0] = SuperManager.Instance.ShopManager.CurrentSkin.BonusBrick;
        mAnimator.runtimeAnimatorController = SuperManager.Instance.ShopManager.CurrentSkin.BonusController;
        base.OnSpawned();
    }
}
