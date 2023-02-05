using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	public SpawnController spawnController;

	public List<Wave> waves = new List<Wave>();

	public GameObject spawnPrefab;
	public GameObject endPrefab;

	public List<Transform> spawns;
	public List<Transform> ends;

	public int life = 5;
	public int money = 30;
	public int actualWaveIndex = 0;

	public bool isDefeat;
	public bool isWaveActive;
	public bool isEndOfWave;

	[SerializeField]
	private TextMeshProUGUI lifeText;
	[SerializeField]
	private TextMeshProUGUI moneyText;

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

		spawnController.SpawnEnnemies();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L) && !isWaveActive)
		{
			spawnController.StartNewWave();
		}
	}

	private void InitUI()
	{
		lifeText.text = life.ToString();
		moneyText.text = money.ToString();
		waveText.text = "";
		waveCompletion.fillAmount = 0;
	}

	private void StartMusic()
	{
		GameManager.Instance.musics.Add(introMusic);
		GameManager.Instance.OnMusicEnded += PlayBattleMusic;
		GameManager.Instance.PlayMusic(true);
	}

	private void PlayBattleMusic()
	{
		GameManager.Instance.OnMusicEnded -= PlayBattleMusic;
		GameManager.Instance.musics = battleMusics;
		GameManager.Instance.PlayMusic();
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
		dataInstance = NavMesh.AddNavMeshData(actualLevel.navMesh);
	}

	public void TakeDamage()
	{
		if (isDefeat)
			return;

		life--;
		lifeText.text = life.ToString();

		if (life <= 0)
			isDefeat = true;
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