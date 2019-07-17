using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	public AudioListener listener;
	public AudioMixerGroup mixer;

	public List<AudioSource> audioSources = new List<AudioSource>();


	void Start()
	{
		listener = Camera.main.GetComponent<AudioListener>();
	}

	void Update()
	{
		for (int i = audioSources.Count-1; i > -1; i--)
		{
			AudioSource source = audioSources[i];
			if(!source.isPlaying)
			{
				audioSources.Remove(source);
				GameObject.Destroy(source);
			}
		}
	}

	public void PlayClip(AudioClip clip)
	{
		listener = Camera.main.GetComponent<AudioListener>();
		AudioSource source = listener.gameObject.AddComponent<AudioSource>();
		source.outputAudioMixerGroup = mixer;
		source.clip = clip;
		source.Play();
		audioSources.Add(source);
	}

	public void PlayRandomClip(AudioClip[] clips)
	{
		listener = Camera.main.GetComponent<AudioListener>();
		if (clips != null && clips.Length > 0)
		{
			AudioClip randomCLip = clips[Random.Range(0, clips.Length)];
			PlayClip(randomCLip);
		}
	}
}