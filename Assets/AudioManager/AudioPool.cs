using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace TapAudio
{
	/// <summary>
	/// Audio Pool manager
	/// </summary>

	[RequireComponent(typeof(AudioManager))]
	public class AudioPool : MonoBehaviour 
	{
		public static AudioPool Instance;

		[SerializeField]
		Transform audioPrefab;

		AudioItem[] items;
		AudioManager AM;

		Dictionary<string, List<Audio>> pool;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake()
		{
			Instance = Instance ?? this;
			AM = GetComponent<AudioManager>();
		}

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		void Start()
		{
			items = AM.GetAudioItems();
			pool = new Dictionary<string, List<Audio>>();

			foreach (var item in items)
			{
				for (int i = 0; i < item.AmountInPool; i++)
				{
					Audio sound = Instantiate(audioPrefab, transform).GetComponent<Audio>();
					sound.SetAudioItem(item);

					if (!pool.ContainsKey(item.Name))
					{
						List<Audio> l = new List<Audio>();
						l.Add(sound);
						pool.Add(item.Name, l);
					}
					else
					{
						var list = pool[item.Name];
						list.Add(sound);
					}
				}
			}
		}

		/// <summary>
		/// Get Audio From Pool 
		/// null if audio not in pool
		/// </summary>
		/// <param name="curName">Name of audio</param>
		/// <returns></returns>
		public Audio GetAudio(string curName)
		{
			if (pool == null)
			{
				Debug.LogError("[AudioPool] Pool is not initialized");
				return null;
			}
			
			if (pool.ContainsKey(curName))
			{
				var l = pool[curName];
				if (l.Count > 0)
				{
					int val = Random.Range(0, l.Count);
					Audio audio = l[val];
					l.RemoveAt(val);
					return audio;
				}
				else
				{
					return null;
				}
			}
			else
			{
				Debug.LogError(string.Format("[AudioPool] Audio {0} not in the pool!", curName));
				return null;
			}
		}

		/// <summary>
		/// Return audio to pool
		/// </summary>
		/// <param name="audio">audio object</param>
		public void ReturnToPool(Audio audio)
		{
			if (pool.ContainsKey(audio.Name))
			{
				List<Audio> l = pool[audio.Name];
				l.Add(audio);
			}
			else
			{
				Debug.LogError(string.Format("[AudioPool] Audio {0} cannot return to pool!", audio.Name));
			}
		}
	}
}
