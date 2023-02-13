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
	public WaveLine waveLine;
	public BattleUI battleUI;

	public List<Wave> waves = new List<Wave>();
	public List<TowersData> towersData = new List<TowersData>();
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
	private ParticleSystem brokenHeartPrefab;

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
		PlayBattleMusic();
		CreateWorld();

		money = actualLevel.initialMoney;
		battleUI.InitUI(money);
	}

	public void PlayBattleMusic()
	{
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

		for (int i = 0 ; i < spawns.Count ; i++)
		{
			for (int j = 0 ; j < ends.Count ; j++)
			{
				waveLine.CreateLine(spawns[i].position, ends[j].position);
			}
		}
	}

	public void LoseLife(Vector3 position)
	{
		life--;
		battleUI.lifeText.text = life.ToString();

		brokenHeartPrefab.transform.position = position + Vector3.up * 2f;
		brokenHeartPrefab.gameObject.SetActive(true);
		brokenHeartPrefab.Play();

		Invoke(nameof(HideHeart), 2f);

		if (life <= 0)
			Lose();
	}

	private void HideHeart()
	{
		brokenHeartPrefab.gameObject.SetActive(false);
	}

	public void GetOrLoseMoney(int value)
	{
		money += value;
		battleUI.moneyText.text = money.ToString();
	}

	public void WaveCompletion(int value, int fromMax)
	{
		battleUI.waveCompletion.fillAmount = DataManager.Instance.Remap(value, 0, fromMax, 0, 1);
	}

	public void ActualWaveDisplay(int waveIndex)
	{
		battleUI.waveText.text = $"Vague {waveIndex}/{waves.Count}";
	}

	public void Victory()
	{
		if (life > 0)
		{
			battleUI.winMenu.SetActive(true);

			if (actualLevel.levelId + 1 > DataManager.Instance.Data.savedWorlds && actualLevel.levelId + 1 < 8)
				DataManager.Instance.SaveData(actualLevel.levelId + 1);
		}
	}

	public void Lose()
	{
		battleUI.loseMenu.SetActive(true);
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
		StartCoroutine(GameManager.Instance.LoadLevel("BattleScene", battleUI.loadingScreen, battleUI.loadingProgress));
	}

	public void Menu()
	{
		StartCoroutine(GameManager.Instance.LoadLevel("MainMenu", battleUI.loadingScreen, battleUI.loadingProgress));
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void PauseUnpauseGame(bool isPausing)
	{
		if (isPausing)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
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