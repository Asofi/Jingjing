using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Skin", menuName = "Skin", order = 1)]
public class Skin : ScriptableObject
{
    public int Cost = 1000;
    
    [Header("Deck")]
    public Sprite Bates;
    public Sprite Ball;
    public Sprite Deck;
    public Sprite Background;
    public Sprite Header;
    [Header("UI")]
    public Sprite Leaderboard;
    public Sprite Sound;
    public Sprite SoundOff;
    public Sprite Share;

    //public Sprite[] CommonBricks;
    [TabGroup("Common")]
    public RuntimeAnimatorController CommonController;
    [TabGroup("Common")]
    public string CommonSound;
    //public Sprite[] SpinnerBricks;
    [TabGroup("Spinner")]
    public RuntimeAnimatorController SpinnerController;
    [TabGroup("Spinner")]
    public string SpinnerSound;
    //public Sprite[] SolidBricks;
    [TabGroup("Solid")]
    public RuntimeAnimatorController SolidController;
    [TabGroup("Solid")]
    public string SolidSound;
    [TabGroup("Solid")]
    public string SolidCrashSound;
    //public Sprite[] StickyBricks;
    [TabGroup("Sticky")]
    public RuntimeAnimatorController StickyController;
    [TabGroup("Sticky")]
    public string StickySound;
    //public Sprite BonusBrick;
    [TabGroup("Bonus")]
    public RuntimeAnimatorController BonusController;
    [TabGroup("Bonus")]
    public string BonusSound;
}
