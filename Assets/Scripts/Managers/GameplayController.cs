using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
	public static GameplayController Instance;

	public SpawnController spawnController;

	public List<Wave> waves = new List<Wave>();

	public Transform endPosition;

	public int healthPoint = 5;
	public int actualWaveIndex = 0;

	public bool isDefeat;
	public bool isWaveActive;
	public bool isEndOfWave;

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

	private void Start()
	{
		spawnController.SpawnEnnemies();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L) && !isWaveActive)
		{
			spawnController.StartNewWave();
		}
	}

	public void TakeDamage()
	{
		if (isDefeat)
			return;

		healthPoint--;

		if (healthPoint <= 0)
			isDefeat = true;
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