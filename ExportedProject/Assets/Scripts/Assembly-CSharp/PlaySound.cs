using System;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
	[Serializable]
	public class ClipInfo
	{
		public string name;

		public AudioClip clip;
	}

	[Serializable]
	public class RandClipInfo
	{
		public string name;

		public bool rollByZero;

		public AudioClip[] clips;

		public AudioSource source;
	}

	public AudioSource source;

	public ClipInfo[] sounds;

	public RandClipInfo[] randSounds;

	private void Start()
	{
	}

	public void Play(string name)
	{
		if (name == string.Empty)
		{
			source.Stop();
			return;
		}
		ClipInfo[] array = sounds;
		foreach (ClipInfo clipInfo in array)
		{
			if (clipInfo.name == name)
			{
				if ((bool)source)
				{
					source.clip = clipInfo.clip;
					source.Play();
				}
				break;
			}
		}
	}

	public void PlayRand(string name)
	{
		RandClipInfo[] array = randSounds;
		foreach (RandClipInfo randClipInfo in array)
		{
			if (!(randClipInfo.name == name))
			{
				continue;
			}
			AudioSource audioSource = (randClipInfo.source ? randClipInfo.source : ((!source) ? null : source));
			if ((bool)audioSource)
			{
				int num = ((!randClipInfo.rollByZero) ? UnityEngine.Random.Range(0, randClipInfo.clips.Length) : UnityEngine.Random.Range(1, randClipInfo.clips.Length));
				audioSource.clip = randClipInfo.clips[num];
				audioSource.Play();
				if (randClipInfo.rollByZero)
				{
					randClipInfo.clips[num] = randClipInfo.clips[0];
					randClipInfo.clips[0] = source.clip;
				}
			}
			break;
		}
	}
}
