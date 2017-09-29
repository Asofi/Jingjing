using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public class AchiviementsManager : MonoBehaviour {

    public static AchiviementsManager Instance;

    const string CUR_STEP_POSTFIX = "_CurStep";

    public Dictionary<string, Achievement> AchieveList = new Dictionary<string, Achievement> {
        {"Rookie", new Achievement("CgkIqNyn1rkEEAIQAg", "00000") },
        {"Amateur", new Achievement("CgkIqNyn1rkEEAIQAw", "00000") },
        {"Pro", new Achievement("CgkIqNyn1rkEEAIQBA", "00000") },
        {"Gamer", new Achievement("CgkIqNyn1rkEEAIQBQ", "00000") },

        {"Skin Collector", new Achievement("CgkIqNyn1rkEEAIQDA", "00000", 0, 3) },

        {"Friendly", new Achievement("CgkIqNyn1rkEEAIQCg", "00000", 0, 5) },

        {"TV-zombie", new Achievement("CgkIqNyn1rkEEAIQCw", "00000", 0, 50) },

        {"Super-Rookie", new Achievement("CgkIqNyn1rkEEAIQBg", "00000") },
        {"Super-Amateur", new Achievement("CgkIqNyn1rkEEAIQBw", "00000") },
        {"Super-Pro", new Achievement("CgkIqNyn1rkEEAIQCQ", "00000") },
        {"Super-Gamer", new Achievement("CgkIqNyn1rkEEAIQAg", "00000") },
    };

    public class Achievement
    {
        public string AndroidID;
        public string IosID;
        public string ID;
        public bool IsUnlocked;
        public int Reward;
        public int Steps = 1;
        private int curStep;
        public float Progress;
        public int CurStep {
            get {
                return curStep;
            }

            set {
                curStep = value;
                Progress = (float)curStep / Steps * 100;
            }
        }

        public Achievement() {
            this.ID = "";
            this.IsUnlocked = false;
            this.Reward = 0;
        }

        /// <summary>
        /// Define your achievement
        /// </summary>
        /// <param name="ID">Achievement ID</param>
        /// <param name="Reward">Reward</param>
        /// <param name="Steps">Max Steps</param>
        public Achievement(string AndroidID, string IosID, int Reward = 0, int Steps = 1)  {
#if UNITY_ANDROID
            ID = AndroidID;
#else
            ID = IosID;
#endif
            this.Steps = Steps;
            IsUnlocked = false;
            CurStep = 0;
            this.Reward = Reward;
        }

    }
	// Use this for initialization
	void Awake () {
        //PlayerPrefs.DeleteAll();
        Instance = Instance ?? this;
        InitAchieves();
	}

    void InitAchieves() {
        foreach(string key in AchieveList.Keys) {
            AchieveList[key].IsUnlocked = PlayerPrefsX.GetBool(key, false);
            AchieveList[key].CurStep = PlayerPrefs.GetInt(key + CUR_STEP_POSTFIX);
        }
    }

    public void UnlockAchivie(string ID) {
        if (AchieveList[ID].IsUnlocked)
            return;
        Social.ReportProgress(AchieveList[ID].ID, 100.0f, (bool success) => {
            AchieveList[ID].IsUnlocked = true;
            PlayerPrefsX.SetBool(ID, true);
            SuperManager.Instance.ShopManager.StarsCount += AchieveList[ID].Reward;
            print("Unlocked " + ID);
        });
    }

    public void IncrementingAchivie(string ID, int Amount = 1) {
        Achievement _achieve = AchieveList[ID];
        if (_achieve.IsUnlocked)
            return;
        int _maxStep = _achieve.Steps;
        int _curStep = _achieve.CurStep;
        if (_curStep < _maxStep) {
            _achieve.CurStep += Amount;
            PlayerPrefs.SetInt(ID + CUR_STEP_POSTFIX, _achieve.CurStep);
        }
        float _progress;
        _progress = _achieve.Progress;

#if UNITY_ANDROID
        PlayGamesPlatform.Instance.IncrementAchievement(
            _achieve.ID, Amount, (bool success) => {
                print("Incrementing " + ID + " from " + _curStep + "/" + _achieve.Steps + " to " + _achieve.CurStep + "/" + _achieve.Steps);
                if (_achieve.CurStep == _maxStep) {
                    _achieve.IsUnlocked = true;
                    PlayerPrefsX.SetBool(ID, true);
                    print("Unlocked " + ID);
                }
            });
#elif UNITY_IOS
        Social.ReportProgress(_achieve.ID, _progress, (bool success) => {
            print(_achieve.CurStep);
            print(_achieve.Steps);
            print("Incrementing " + ID + " to " + _progress + "/100");
            if (_progress == 100) {
                AchieveList[ID].IsUnlocked = true;
                PlayerPrefsX.SetBool(ID, true);
                print("Unlocked " + ID);
            }
            SuperManager.Instance.ShopManager.StarsCount += AchieveList[ID].Reward;
        });
#endif

    }
}
