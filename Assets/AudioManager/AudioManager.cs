using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

using TapAudio;

[RequireComponent(typeof(AudioPool))]
public class AudioManager : MonoBehaviour 
{
    public const string SOUND_TOGGLE = "sound_toggle";
    public static AudioManager Instance;
    public bool Mute { private set; get; }

    public Toggle SoundToggle;

    [SerializeField] int WaitHalfOfSeconds = 4;

    const string nameOfDir = "AudioItems";    
    Dictionary<string, AudioItem> AudioCollection;    
    Dictionary<string, Audio> AudioUnderControll;

    [Serializable]
    public struct structSound 
    {
        public AudioClip sound;
        public string name;   
    }

    structSound[] Sounds;
    AudioPool pool;

    private void Awake() 
    {
        pool = GetComponent<AudioPool>();
        AudioManager.Instance = AudioManager.Instance ?? this;
        BuildCollection();
        Mute = PlayerPrefs.GetInt(SOUND_TOGGLE) == 1;
        SoundToggle.isOn = Mute;
        AudioListener.volume = Mute ? 0 : 1;
        AudioUnderControll = new Dictionary<string, Audio>();
    }

    /// <summary>
    /// Mute or unmute
    /// </summary>
    /// <param name="mute"></param>
    public static void SelectMute(bool mute) 
    {
        Instance.Mute = mute;
        PlayerPrefs.SetInt(SOUND_TOGGLE, Instance.Mute == true ? 1 : 0);
        AudioListener.volume = Instance.Mute ? 0 : 1;        
    }

    public void SelectMuteToggle(bool mute) {
        Instance.Mute = mute;
        PlayerPrefs.SetInt(SOUND_TOGGLE, Instance.Mute == true ? 1 : 0);
        AudioListener.volume = Instance.Mute ? 0 : 1;
    }

    /// <summary>
    /// Play audio and follow him
    /// Warning! Get music only in looping!
    /// </summary>
    /// <param name="audioName">Audio Name</param>
    /// <param name="CommonName">Name in controllable list, if null set default audio name</param>
    public void PlayControllable(string audioName, string commonName = null, bool waitIfNotAviable = false)
    {
       StartCoroutine(GetAndPlayAudioControllable(audioName, commonName, waitIfNotAviable));
    }

    public static void PlayAudioControllable(string audioName, string commonName = null, bool waitIfNotAviable = false) 
    {
        Instance.PlayControllable(audioName, commonName, waitIfNotAviable);
    }

    public void Stop(string audioName)
    {
        if (AudioUnderControll.ContainsKey(audioName))
        {
            AudioUnderControll[audioName].Stop();
            AudioUnderControll.Remove(audioName);
        }
    }

    public static void PlayAudio(string audioName)
    {
        Instance.Play(audioName);
    }

    public AudioItem[] GetAudioItems()
    {
        return AudioCollection.Values.ToArray();
    }
    
    /// <summary>
    /// Collect all sound from resources
    /// </summary>
    public void CollectAudios() 
    {
        AudioClip[] audios = Resources.LoadAll<AudioClip>("Sounds");
        Sounds = new structSound[audios.Length];
        for (int i = 0; i < audios.Length; i++) 
        {
            Sounds[i] = new structSound {sound = audios[i], name = audios[i].name};
        }
        MakeAudioCollection();
    }

    private void MakeAudioCollection()
    {
        Directory.CreateDirectory(string.Format("Assets/Resources/{0}", nameOfDir));
        AudioItem[] Audios = Resources.LoadAll<AudioItem>(nameOfDir);
        AudioCollection = new Dictionary<string, AudioItem>();
        
        foreach (var a in Audios)
        {
            AudioCollection.Add(a.FileName, a);
        }

        #if UNITY_EDITOR
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (!AudioCollection.ContainsKey(Sounds[i].name))
            {
                AudioItem asset = ScriptableObject.CreateInstance<AudioItem>();
                asset.Name = Sounds[i].name;
                asset.FileName = Sounds[i].name;
                asset.Clip = Sounds[i].sound;
                AssetDatabase.CreateAsset(asset, 
                                            String.Format(
                                                "Assets/Resources/{0}/Audio-{1}.asset", 
                                                nameOfDir, 
                                                Sounds[i].name)
                                        );
                AssetDatabase.SaveAssets();
            }
        }
        #endif
    }

    private void BuildCollection()
    {
        AudioItem[] Audios = Resources.LoadAll<AudioItem>(nameOfDir);
        AudioCollection = new Dictionary<string, AudioItem>();
        
        foreach (var a in Audios)
        {
            AudioCollection.Add(a.FileName, a);
        }
    }

    public void Play(string audioName)
    {
        Audio audio = pool.GetAudio(audioName);
        if (audio != null)
        {
            audio.Play();
        }
    }

    IEnumerator GetAndPlayAudioControllable(string audioName, string commonName, bool waitIfNotAviable)
    {
        Stop(commonName == null ? audioName : commonName);
        Audio audio = pool.GetAudio(audioName);
        if (waitIfNotAviable && audio == null)
        {
            for (int i = 0; i < WaitHalfOfSeconds; i++)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                audio = pool.GetAudio(audioName);
                if (audio != null)
                    break;
            }
        }
        if (audio != null)
        {
            audioName = commonName == null ? audioName : commonName;
            AudioUnderControll.Add(audioName, audio);
            audio.Play();
        }
        else
        {
            if (waitIfNotAviable)
                Debug.LogError("[AudioManager] Audio Cannot be loaded");
        }
    }

}
