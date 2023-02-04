using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public AudioSource audioSource;
	public List<AudioClip> musics = new List<AudioClip>();
	public event Action OnMusicEnded;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
			Instance = this;

		DontDestroyOnLoad(gameObject);
	}

	public void PlayMusic(bool isMusicEnded = false, bool isLoop = false)
	{
		if (musics.Count == 0)
			return;

		int musicIndex = UnityEngine.Random.Range(0, musics.Count);
		audioSource.clip = musics[musicIndex];
		audioSource.Play();

		if (musics.Count > 1 || isLoop)
			Invoke(nameof(PlayMusic), musics[musicIndex].length + 1f);

		if (isMusicEnded)
			Invoke(nameof(MusicEnded), musics[musicIndex].length + 1f);
	}

	private void MusicEnded()
	{
		OnMusicEnded?.Invoke();
	}

	public void StopMusic()
	{
		CancelInvoke();
		musics.Clear();
		audioSource.Stop();
	}
}