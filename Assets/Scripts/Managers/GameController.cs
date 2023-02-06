using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	public SpawnController spawnController;

	public List<Wave> waves = new List<Wave>();
	public List<Towers> towers = new List<Towers>();

	public List<Transform> spawns;
	public List<Transform> ends;

	public int life = 5;
	public int money = 30;
	public int actualWaveIndex = 0;

	[SerializeField]
	private GameObject spawnPrefab;
	[SerializeField]
	private GameObject endPrefab;

	[SerializeField]
	private TextMeshProUGUI lifeText;
	[SerializeField]
	private TextMeshProUGUI moneyText;

	public GameObject nextWaveButton;
	[SerializeField]
	private GameObject winMenu;
	[SerializeField]
	private GameObject loseMenu;

	[SerializeField]
	private TextMeshProUGUI waveText;
	[SerializeField]
	private Image waveCompletion;

	[SerializeField]
	private AudioClip introMusic;
	[SerializeField]
	private List<AudioClip> battleMusics;

	private LevelsData actualLevel;
	private NavMeshDataInstance dataInstance;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
			Instance = this;
	}

	private void Start()
	{
		StartMusic();
		CreateWorld();
		InitUI();
	}

	private void StartMusic()
	{
		GameManager.Instance.musics.Clear();
		GameManager.Instance.musics.Add(introMusic);
		GameManager.Instance.OnMusicEnded += PlayBattleMusic;
		StartCoroutine(GameManager.Instance.PlayMusic());
	}

	private void PlayBattleMusic()
	{
		GameManager.Instance.OnMusicEnded -= PlayBattleMusic;
		GameManager.Instance.musics = battleMusics;
		StartCoroutine(GameManager.Instance.PlayMusic());
	}

	private void CreateWorld()
	{
		actualLevel = DataManager.Instance.actualLevel;

		GameObject world = Instantiate(actualLevel.map);
		spawns.Clear();
		ends.Clear();

		for (int i = 0 ; i < actualLevel.spawn.Count ; i++)
		{
			GameObject spawn = Instantiate(spawnPrefab);
			spawn.transform.position = actualLevel.spawn[i];
			spawns.Add(spawn.transform);
		}

		for (int i = 0 ; i < actualLevel.end.Count ; i++)
		{
			GameObject end = Instantiate(endPrefab);
			end.transform.position = actualLevel.end[i];
			ends.Add(end.transform);
		}

		waves = actualLevel.waves;
		NavMesh.RemoveAllNavMeshData();
		dataInstance = NavMesh.AddNavMeshData(actualLevel.navMesh);
	}

	private void InitUI()
	{
		lifeText.text = life.ToString();
		money = actualLevel.initialMoney;
		moneyText.text = money.ToString();
		waveText.text = "";
		waveCompletion.fillAmount = 0;
	}

	public void TakeDamage()
	{
		life--;
		lifeText.text = life.ToString();

		if (life <= 0)
			Lose();
	}

	public void GetOrLoseMoney(int value)
	{
		money += value;
		moneyText.text = money.ToString();
	}

	public void WaveCompletion(int value, int fromMax)
	{
		waveCompletion.fillAmount = DataManager.Instance.Remap(value, 0, fromMax, 0, 1);
	}

	public void ActualWaveDisplay(int waveIndex)
	{
		waveText.text = $"Vague {waveIndex}/{waves.Count}";
	}

	public void Victory()
	{
		if (life > 0)
		{
			winMenu.SetActive(true);

			if (actualLevel.levelId + 1 > DataManager.Instance.Data.savedWorlds && actualLevel.levelId + 1 < 8)
				DataManager.Instance.SaveData(actualLevel.levelId + 1);
		}
	}

	public void Lose()
	{
		loseMenu.SetActive(true);
	}

	public void Continue()
	{
		if (actualLevel.levelId + 1 < 9)
		{
			DataManager.Instance.actualLevel = DataManager.Instance.allLevels[actualLevel.levelId + 1];
			SceneManager.LoadScene("BattleScene");
		}
	}

	public void Retry()
	{
		SceneManager.LoadScene("BattleScene");
	}

	public void Menu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void Quit()
	{
		Application.Quit();
	}
}

[Serializable]
public class Wave
{
	public List<TypeOfEnnemies> ennemiesType;
}

[Serializable]
public class TypeOfEnnemies
{
	public EnnemiesData data;
	public int nbrToSpawn;
}