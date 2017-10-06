using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour {
    private void Start() {
//#if UNITY_ANDROID
//        Advertisement.Initialize("1434477", false);
//#else
//        Advertisement.Initialize("1434478", false);
//#endif
    }

    public void ShowRewardedAd() {
        if (Advertisement.IsReady("rewardedVideo")) {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result) {
        switch (result) {
            case ShowResult.Finished:
                AchiviementsManager.Instance.IncrementingAchivie("TV-zombie");
                Debug.Log("The ad was successfully shown.");
                SuperManager.Instance.GUIManager.SecondChanceButton();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}
