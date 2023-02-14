using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller of the audio and musics of the game
/// </summary>
public class AudioController : MonoBehaviour
{
	public event Action OnMusicEnded;

	private AudioSource musicSource;
	private AudioSource fadeSource;
	private List<AudioClip> musics = new List<AudioClip>();

	private bool isFadeUp;
	private bool isFadingVolume;

	private void Awake()
	{
		musicSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		AudioListener.volume = DataManager.Instance.Data.volume;
	}

	private void Update()
	{
		if (isFadingVolume)
			FadeVolume();
	}

	/// <summary>
	/// Start playing music
	/// </summary>
	/// <param name="clips">List of audio clips to play</param>
	public void StartPlayMusic(List<AudioClip> clips)
	{
		if (musicSource == null)
		{
			Debug.LogError($"No AudioSource found on {name} in AudioController.cs !");
			return;
		}

		musics = clips;
		StartCoroutine(PlayMusic());
	}

	/// <summary>
	/// Stop playing musics
	/// </summary>
	public void StopMusic()
	{
		CancelInvoke();
		musics.Clear();
		musicSource.Stop();
	}

	/// <summary>
	/// Play music, and if there's more than one clip, play again an other one
	/// </summary>
	private IEnumerator PlayMusic()
	{
		if (musics.Count > 0)
		{
			int musicIndex = UnityEngine.Random.Range(0, musics.Count);
			musicSource.clip = musics[musicIndex];
			musicSource.Play();

			yield return new WaitForSeconds(musics[musicIndex].length + 1f);

			if (musics.Count > 1)
				StartCoroutine(PlayMusic());
			else
				OnMusicEnded?.Invoke();
		}
	}

	/// <summary>
	/// Fade the volume of an AudioSource up or down
	/// </summary>
	/// <param name="source">AudioSource to fade up or down</param>
	/// <param name="isFadeUp">If true : fade the volume from 0 to 1</param>
	public void StartFadeVolume(AudioSource source, bool isFadeUp = false)
	{
		if (isFadingVolume)
		{
			Debug.LogError("An AudioSource is already fading, in AudioController.cs !");
			return;
		}

		if (isFadeUp)
			source.volume = 0f;
		else
			source.volume = 1f;

		fadeSource = source;
		isFadingVolume = true;
		this.isFadeUp = isFadeUp;
	}

	/// <summary>
	/// Fade the volume
	/// </summary>
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
}