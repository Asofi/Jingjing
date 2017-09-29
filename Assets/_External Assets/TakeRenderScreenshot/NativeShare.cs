using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

public class NativeShare : MonoBehaviour {
    public string ScreenshotName = "Screenshot.png";

	[Tooltip("Size of screen")]
	public int ScreenWidth = 1080;
    public int ScreenHeight = 1920;

	[Tooltip("Camera which will make screen")]
    public Camera AttachedCamera;

	/// <summary>
	/// Save screenshot to texture
	/// </summary>
	public void SaveTexture()
    {
        StartCoroutine(saveTexture());
    }

	/// <summary>
	/// Begin screenshot share
	/// </summary>
    public void StartSharing(string str)
    {
		StartCoroutine(delayedShare(str));
    }


	/// <summary>
	/// CaptureScreenshot runs asynchronously, so you'll need to either capture the screenshot early and wait a fixed time
    /// for it to save, or set a unique image name and check if the file has been created yet before sharing.
	/// </summary>
	/// <param name="text">Help text string</param>
	/// <returns>IEnumerator</returns>
    public IEnumerator delayedShare(string text)
    {
        yield return null;

        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        Share(text, screenShotPath, "");
    }

	private void Share(string shareText, string imagePath, string url, string subject = "")
	{
		#if UNITY_ANDROID
			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
			intentObject.Call<AndroidJavaObject>("setType", "image/png");

			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
			currentActivity.Call("startActivity", jChooser);
		#elif UNITY_IOS
			CallSocialShareAdvanced(shareText, subject, url, imagePath);
		#else
			Debug.Log("No sharing set up for this platform.");
		#endif
	}

  	private Texture2D FillInClear(Texture2D tex2D, Color whatToFillWith)
    {
        for (int i = 0; i < tex2D.width; i++)
        {
            for (int j = 0; j < tex2D.height; j++)
            {
                if (tex2D.GetPixel(i, j) == Color.clear)
                    tex2D.SetPixel(i, j, whatToFillWith);
            }
        }
        return tex2D;
    }

    private IEnumerator saveTexture()
    {
        yield return null;
        AttachedCamera.enabled = true;
        RenderTexture rt = new RenderTexture(ScreenWidth, ScreenHeight, 24, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Point;
        AttachedCamera.targetTexture = rt;
        Texture2D newTexture = new Texture2D(AttachedCamera.targetTexture.width, AttachedCamera.targetTexture.height, TextureFormat.RGB24, false);
        AttachedCamera.Render();
        RenderTexture.active = rt;
        newTexture.ReadPixels(new Rect(0, 0, AttachedCamera.targetTexture.width, AttachedCamera.targetTexture.height), 0, 0, false);
        RenderTexture.active = null;
        DestroyImmediate(rt);
        AttachedCamera.enabled = false;
        byte[] bytes = newTexture.EncodeToPNG();
#if UNITY_EDITOR
        File.WriteAllBytes("Screenshot.png", bytes);
#else
            File.WriteAllBytes(Application.persistentDataPath + "/Screenshot.png", bytes);
#endif
    }

#if UNITY_IOS
		public struct ConfigStruct
		{
			public string title;
			public string message;
		}

		[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

		public struct SocialSharingStruct
		{
			public string text;
			public string url;
			public string image;
			public string subject;
		}

		[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);

		public static void CallSocialShare(string title, string message)
		{
			ConfigStruct conf = new ConfigStruct();
			conf.title  = title;
			conf.message = message;
			showAlertMessage(ref conf);
		}


		public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
		{
			SocialSharingStruct conf = new SocialSharingStruct();
			conf.text = defaultTxt;
			conf.url = url;
			conf.image = img;
			conf.subject = subject;

			showSocialSharing(ref conf);
		}
#endif


}
