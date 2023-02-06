using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private Transform btnsParent;
	[SerializeField]
	private Transform levelsParent;
	[SerializeField]
	private Transform optionsParent;

	[SerializeField]
	private List<Button> btnLevels;
	[SerializeField]
	private List<GameObject> lockLevels;

	[SerializeField]
	private List<AudioClip> menuMusics;

	[SerializeField]
	private Slider volumeSlider;

	private void Awake()
	{
		btnsParent.gameObject.SetActive(true);
		levelsParent.gameObject.SetActive(false);
		optionsParent.gameObject.SetActive(false);
	}

	private void Start()
	{
		GameManager.Instance.musics = menuMusics;
		StartCoroutine(GameManager.Instance.PlayMusic());

		UnlockedLevel();
		InitializeOptions();
	}

	private void UnlockedLevel()
	{
		for (int i = 1 ; i < btnLevels.Count ; i++)
		{
			if (i < DataManager.Instance.Data.savedWorlds + 1)
			{
				lockLevels[i].SetActive(false);

				int levelIndex = i;
				btnLevels[i].onClick.AddListener(delegate
				{
					LoadBattleScene(levelIndex);
				});
			}
			else
			{
				lockLevels[i].SetActive(true);
				btnLevels[i].onClick.RemoveAllListeners();
			}
		}
	}

	private void InitializeOptions()
	{
		volumeSlider.value = DataManager.Instance.Data.volume;
	}

	public void ChangeVolume(float value)
	{
		AudioListener.volume = value;
		DataManager.Instance.Data.volume = value;
		DataManager.Instance.SaveData(-1, value);
	}

	public void LoadBattleScene(int levelIndex)
	{
		GameManager.Instance.StopMusic();
		DataManager.Instance.actualLevel = DataManager.Instance.allLevels[levelIndex];
		SceneManager.LoadScene("BattleScene");
	}
}