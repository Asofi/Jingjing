using UnityEngine;

/// <summary>
/// FPS Counter running selector
/// </summary>

public class FPSController : MonoBehaviour {

	public bool DontDestoryInRelease = false;	/// Don't destroy FPS Counter in Release build
	public bool DestroyInDebug = false;			/// Destroy FPS Counter in Debug build		
	public bool RunInEditor = false;			/// Run FPS Counter in Editor 

	private GameObject FPSCounterObj;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake() {
		FPSCounterObj = transform.GetChild(0).gameObject;

		bool isNeedToDestroy = false;
		bool isEditor = false;

		#if UNITY_EDITOR
		isEditor = true;
		#endif
		
		if (Debug.isDebugBuild) 
		{
			if (DestroyInDebug)
				isNeedToDestroy = true;
		} 
		else 
		{
			if (!DontDestoryInRelease) 
				isNeedToDestroy = true;
		}

		if (isNeedToDestroy) 
		{
			Destroy(gameObject);
		}
		else 
		{
			if (!isEditor || isEditor && RunInEditor)
				FPSCounterObj.SetActive(true);
		}
	}	
}