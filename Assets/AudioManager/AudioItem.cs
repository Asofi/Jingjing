using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Audio", menuName = "AudioItem", order = 1)]
public class AudioItem : ScriptableObject 
{
	public string Name = string.Empty;							/// Name of sound
	public int AmountInPool = 3;								/// How much can be contained in pool
	[Range(0,1)] public float Volume = 1;						/// Volume of Audio source
	public bool IsLooped = false;								/// Play in loop?
	[HideInInspector] public AudioClip Clip;					/// AudioClip
	[HideInInspector] public string FileName = string.Empty;	/// Real file name
}