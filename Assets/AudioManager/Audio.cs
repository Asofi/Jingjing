using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapAudio
{
	/// <summary>
	/// Runtime Audio Object
	/// Provide functions to controll audio
	/// </summary>

	[RequireComponent(typeof(AudioSource))]
	public class Audio : MonoBehaviour 
	{
		/// Name of sound in pool
		public string Name { private set; get; }

		AudioSource mAudioSource;
		Coroutine mCoroutinePlaying;
		bool isPaused = false;
		
		/// <summary>
		/// Set audio properties from scriptable object
		/// </summary>
		/// <param name="item">scriptable item</param>
		public void SetAudioItem(AudioItem item)
		{
			mAudioSource = GetComponent<AudioSource>();	
			Name = item.Name;	
			mAudioSource.clip = item.Clip;
			mAudioSource.volume = item.Volume;
			mAudioSource.loop = item.IsLooped;
		}

		/// <summary>
		/// Play sound once
		/// </summary>
		public void Play()
		{
			if (mCoroutinePlaying == null)
				mCoroutinePlaying =	StartCoroutine(Playing());
			else
				Debug.LogError(string.Format("Double Audio {0} playing", Name));
		}

		/// <summary>
		/// Stop sound
		/// </summary>
		public void Stop()
		{
			mAudioSource.Stop();
			isPaused = false;
		}

		/// <summary>
		/// Play in loop force
		/// </summary>
		// public void PlayLoop()
		// {
		// 	mAudioSource.loop = true;
		// 	Play();
		// }

		// /// <summary>
		// /// Stop and switch off looping
		// /// </summary>
		// public void StopLoop()
		// {
		// 	mAudioSource.loop = false;
		// 	Stop();
		// }

		/// <summary>
		/// Pause 
		/// </summary>
		public void Pause()
		{
			isPaused = true;
			mAudioSource.Pause();
		}

		/// <summary>
		/// UnPause sound
		/// </summary>
		public void UnPause()
		{
			isPaused = false;
			mAudioSource.UnPause();
		}

		IEnumerator Playing()
		{
			mAudioSource.Play();
			yield return new WaitWhile(() => mAudioSource.isPlaying && !isPaused); 
			Die();
		}

		void Die()
		{
			mCoroutinePlaying = null;
			AudioPool.Instance.ReturnToPool(this);
		}
	}
}
