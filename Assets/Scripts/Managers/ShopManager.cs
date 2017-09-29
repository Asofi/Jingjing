using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

    public static ShopManager Instance;

    [Header("Skins")]
    public Skin[] Skins;
    public bool[] UnlockedSkins;
    public Skin currentSkin;
    private int curSkinNum = 0;

    public SpriteRenderer Deck, BG, LeftBate, RightBate, Header;
    public GameObject[] FirstSkinBalls;
    public GameObject[] SecondSkinBalls;
    public GameObject[] ChemAnim;

    public Image Leaderboard, Sound, Share, SoundOff;
    public Text Cost;

    private const string STARS = "stars";
    private const string CHOOSED_SKIN = "choosed_skin";
    private const string UNLOCKED_SKINS = "unlocked_skins";

    private int bougthSkins = 0;
    private int starsCount;

    public Image[] SkinIndicators;

    public int StarsCount
    {
        get
        {
            return starsCount;
        }

        set
        {
            starsCount = value;
            SuperManager.Instance.GUIManager.ChangeText(SuperManager.Instance.GUIManager.Stars, starsCount.ToString());
        }
    }

    public Skin CurrentSkin
    {
        get
        {
            return currentSkin;
        }

        set
        {
            if(currentSkin != value)
            {
                currentSkin = value;
                EventManager.ChangeSkin(currentSkin);
            }
        }
    }

    // Use this for initialization
    void Awake () {
        Instance = this;
        starsCount = PlayerPrefs.GetInt(STARS, 0);
        CurrentSkin = Skins[0];
        UnlockedSkins = PlayerPrefsX.GetBoolArray(UNLOCKED_SKINS, false, 3);
        UnlockedSkins[0] = true;
        SkinChange(PlayerPrefs.GetInt(CHOOSED_SKIN));

        foreach (bool b in UnlockedSkins) {
            if(b) {
                bougthSkins = Mathf.Clamp(bougthSkins++, 0, 3);
                //AchiviementsManager.Instance.IncrementingAchivie("Skin Collector", 1);
            }
        }
    }

    private void Start()
    {
        SuperManager.Instance.GUIManager.ChangeText(SuperManager.Instance.GUIManager.Stars, starsCount.ToString());
        EventManager.OnAddCoin += EventManager_OnAddCoin;
    }

    private void EventManager_OnAddCoin()
    {
        StarsCount++;
        PlayerPrefs.SetInt(STARS, StarsCount);
    }

    public void SkinChange(int num)
    {
        curSkinNum = num;
        if (num != 0)
            foreach (GameObject go in FirstSkinBalls)
                go.SetActive(false);
        else
            foreach (GameObject go in FirstSkinBalls)
                go.SetActive(true);

        if(num != 1)
            foreach (GameObject go in SecondSkinBalls)
                go.SetActive(false);
        else
            foreach (GameObject go in SecondSkinBalls)
                go.SetActive(true);

        switch (num)
        {
            case 0:
                SkinIndicators[0].enabled = true;
                SkinIndicators[1].enabled = false;
                SkinIndicators[2].enabled = false;
                foreach (GameObject ChemAnim in ChemAnim)
                    ChemAnim.SetActive (false);
                break;
            case 1:
                SkinIndicators[0].enabled = false;
                SkinIndicators[1].enabled = true;
                SkinIndicators[2].enabled = false;
                Cost.text = 500.ToString();
                foreach (GameObject ChemAnim in ChemAnim)
                    ChemAnim.SetActive(false);
                break;
            case 2:
                SkinIndicators[0].enabled = false;
                SkinIndicators[1].enabled = false;
                SkinIndicators[2].enabled = true;
                Cost.text = 1000.ToString();
                foreach (GameObject ChemAnim in ChemAnim)
                    ChemAnim.SetActive(true);
                break;
        }


        PlayerPrefs.SetInt(CHOOSED_SKIN, num);
        CurrentSkin = Skins[num];
        Deck.sprite = CurrentSkin.Deck;
        BG.sprite = CurrentSkin.Background;
        LeftBate.sprite = CurrentSkin.Bates;
        RightBate.sprite = CurrentSkin.Bates;
        Header.sprite = CurrentSkin.Header;
        SoundOff.sprite = CurrentSkin.SoundOff;

        Leaderboard.sprite = CurrentSkin.Leaderboard;
        Sound.sprite = CurrentSkin.Sound;
        Share.sprite = CurrentSkin.Share;


        SuperManager.Instance.GUIManager.SkinChange(UnlockedSkins[num]);
    }

    public void BuySkin()
    {
        if(StarsCount + 1 <= Skins[curSkinNum].Cost)
        {
            print("Farm more stars!");
            return;
        }
        StarsCount -= Skins[curSkinNum].Cost;
        UnlockedSkins[curSkinNum] = true;
        PlayerPrefsX.SetBoolArray(UNLOCKED_SKINS, UnlockedSkins);
        PlayerPrefs.SetInt(STARS, StarsCount);
        SuperManager.Instance.GUIManager.SkinChange(UnlockedSkins[curSkinNum]);
        bougthSkins = Mathf.Clamp(bougthSkins++, 0, 3);
        AchiviementsManager.Instance.IncrementingAchivie("Skin Collector");
        //if (bougthSkins == 3)
        //    AchiviementsManager.Instance.UnlockAchivie("Skin Collector");
    }
}
