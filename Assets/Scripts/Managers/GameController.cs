using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	public LevelsData actualLevel;

	public SpawnController spawnController;

	public List<Wave> waves = new List<Wave>();

	public Transform spawn;
	public Transform end;

	public int healthPoint = 5;
	public int actualWaveIndex = 0;

	public bool isDefeat;
	public bool isWaveActive;
	public bool isEndOfWave;

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

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		CreateWorld();
		spawnController.SpawnEnnemies();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L) && !isWaveActive)
		{
			spawnController.StartNewWave();
		}
	}

	private void CreateWorld()
	{
		GameObject world = Instantiate(actualLevel.map);
		spawn.position = actualLevel.spawn;
		end.position = actualLevel.end;
		waves = actualLevel.waves;

		dataInstance = NavMesh.AddNavMeshData(actualLevel.navMesh);
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