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

	private AudioSource fadeSource;
	private bool isFadeUp;
	private bool isFadingVolume;

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

	private void Update()
	{
		if (isFadingVolume)
			FadeVolume();
	}

	public IEnumerator PlayMusic(bool isLoop = false)
	{
		if (musics.Count > 0)
		{
			int musicIndex = UnityEngine.Random.Range(0, musics.Count);
			audioSource.clip = musics[musicIndex];
			audioSource.Play();

			yield return new WaitForSeconds(musics[musicIndex].length + 1f);

			if (musics.Count > 1 || isLoop)
				StartCoroutine(PlayMusic(isLoop));
			else
				OnMusicEnded?.Invoke();
		}
	}

	public void StopMusic()
	{
		CancelInvoke();
		musics.Clear();
		audioSource.Stop();
	}

	public void StartFadeVolume(AudioSource source, bool isFadeUp = false)
	{
		if (isFadeUp)
			source.volume = 0f;
		else
			source.volume = 1f;

		fadeSource = source;
		isFadingVolume = true;
		this.isFadeUp = isFadeUp;
	}

	private void FadeVolume()
	{
		if (fadeSource != null)
		{
			if (isFadeUp)
			{
				if (fadeSource.volume >= 1f)
				{
					isFadingVolume = false;
					fadeSource = null;
					return;
				}

				fadeSource.volume += Time.deltaTime * 0.5f;
			}
			else
			{
				if (fadeSource.volume <= 0f)
				{
					isFadingVolume = false;
					fadeSource = null;
					return;
				}

				fadeSource.volume -= Time.deltaTime * 0.5f;
			}
		}
	}

	public void Quit()
	{
		Application.Quit();
	}
}