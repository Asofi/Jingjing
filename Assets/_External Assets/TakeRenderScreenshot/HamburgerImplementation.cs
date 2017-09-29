using UnityEngine;

// private field assigned but not used.
#pragma warning disable 0414 


public class HamburgerImplementation : MonoBehaviour {
    [Tooltip("URL will be added to text in function")]
    [SerializeField] private string AndroidId;
    [SerializeField] private string IosId;
    [Tooltip("Other Games Link")]
    [SerializeField] private string OtherGamesLink = "http://fasttap.io";

    public UnityEngine.UI.Text ScreenBestScore;

    [TextArea]
    [Tooltip("Text without App URL")]
    [SerializeField] private string TextWithoutURL;

    [SerializeField] private Canvas CanvasToShow;

    public static HamburgerImplementation Instance;
    private NativeShare nativeShare;

    private string AndroidAppStoreUrl = "https://play.google.com/store/apps/details?id=";
    private string IOSAppStoreUrl = "https://itunes.apple.com/app/id";
    private string AndroidAppRateUrl = "market://details?id=";
    private string IOSAppRateUrl = "itms-apps://itunes.apple.com/app/id";

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Instance = Instance ?? this;
        nativeShare = GetComponent<NativeShare>();
        //EnableCanvas(false);
    }

    /// <summary>
    /// Button event
    /// </summary>
    public void ShareScreen() 
    {
        //EnableCanvas(true);
        AchiviementsManager.Instance.IncrementingAchivie("Friendly");
        ScreenBestScore.text = PlayerPrefs.GetInt("best_score").ToString();
        nativeShare.SaveTexture();
#if UNITY_ANDROID
        nativeShare.StartSharing(TextWithoutURL + " " + AndroidAppStoreUrl + AndroidId);
#elif UNITY_IOS
        	nativeShare.StartSharing(TextWithoutURL + " " + IOSAppStoreUrl + IosId);
#endif

        //EnableCanvas(false);
    }

    /// <summary>
    /// Open Rate Link
    /// </summary>
    public void OpenRateLink()
    {
        #if UNITY_ANDROID
            Application.OpenURL(AndroidAppRateUrl + AndroidId);
        #elif UNITY_IOS
            Application.OpenURL(IOSAppRateUrl + IosId);
        #endif
    }

    /// <summary>
    /// Open Other Games Link
    /// </summary>
    public void OpenOtherGamesLink()
    {
        Application.OpenURL(OtherGamesLink);
    }

    private void EnableCanvas(bool enable) 
    {
        if (CanvasToShow != null) 
        {
            CanvasToShow.enabled = false;
        }
    }
}
