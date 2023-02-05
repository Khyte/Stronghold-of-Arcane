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
		GameManager.Instance.PlayMusic();
		UnlockedLevel();
		InitializeOptions();
	}

	private void UnlockedLevel()
	{
		for (int i = 0 ; i < btnLevels.Count ; i++)
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

		/*for (int i = 0 ; i < DataManager.Instance.Data.savedWorlds + 1 ; i++)
		{
			Button button;

			int y;

			if ((i + 1) % 3 == 0)
				y = -280;
			else if ((i + 2) % 3 == 0)
				y = -100;
			else
				y = 80;

			if (i > 2 && i < 6)
			{
				button = Instantiate(btnLevelPrefabs[1], levelsParent);

				if (DataManager.Instance.Data.savedWorlds > 5)
					button.transform.localPosition = new Vector3(0, y, 0);
				else
					button.transform.localPosition = new Vector3(200, y, 0);
			}
			else if (i > 5)
			{
				button = Instantiate(btnLevelPrefabs[2], levelsParent);
				button.transform.localPosition = new Vector3(400, y, 0);
			}
			else
			{
				button = Instantiate(btnLevelPrefabs[0], levelsParent);

				if (DataManager.Instance.Data.savedWorlds > 5)
					button.transform.localPosition = new Vector3(-400, y, 0);
				else if (DataManager.Instance.Data.savedWorlds > 2)
					button.transform.localPosition = new Vector3(-200, y, 0);
				else
					button.transform.localPosition = new Vector3(0, y, 0);
			}

			button.GetComponentInChildren<TextMeshProUGUI>().text = $"Niveau 1.{i + 1}";

			int levelIndex = i;
			button.onClick.AddListener(delegate
			{
				LoadBattleScene(levelIndex);
			});
		}*/
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

	private void LoadBattleScene(int levelIndex)
	{
		GameManager.Instance.StopMusic();
		DataManager.Instance.actualLevel = DataManager.Instance.allLevels[levelIndex];
		SceneManager.LoadScene("BattleScene");
	}
}