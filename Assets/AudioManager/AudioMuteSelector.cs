using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapAudio
{
	/// <summary>
	/// Mute selector
	/// </summary>

	[RequireComponent(typeof(AudioListener))]
	public class AudioMuteSelector : MonoBehaviour 
	{
		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake()
		{
			bool isMute = PlayerPrefs.GetInt(AudioManager.SOUND_TOGGLE, 0) == 1;
			AudioListener.volume = isMute ? 0 : 1;
		}
	}
}
