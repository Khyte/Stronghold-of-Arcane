using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	private List<Wave> waves;
	private Wave actualWave;

	private List<EnnemiesData> waveEnnemies;
	private List<Ennemies> ennemiesPool = new List<Ennemies>();

	private int actualEnnemyIndex;
	private int maxEnnemies;

	private void InitializeEnnemy(Ennemies ennemy, EnnemiesData data)
	{
		ennemy.data = data;
		ennemy.name = data.name;
		ennemy.InitializeEnnemy();
	}

	public void SpawnEnnemies()
	{
		waves = GameController.Instance.waves;

		for (int i = 0 ; i < waves[0].ennemiesType.Count ; i++)
		{
			for (int j = 0 ; j < waves[0].ennemiesType[i].nbrToSpawn ; j++)
			{
				GameObject ennemy = Instantiate(waves[0].ennemiesType[i].data.prefab, Vector3.zero, Quaternion.identity, null);

				Ennemies ennemyComp = ennemy.GetComponent<Ennemies>();
				InitializeEnnemy(ennemyComp, waves[0].ennemiesType[i].data);

				ennemy.SetActive(false);
				ennemiesPool.Add(ennemyComp);
			}
		}
	}

	public void StartNewWave()
	{
		if (GameController.Instance.actualWaveIndex >= waves.Count)
			return;

		actualWave = waves[GameController.Instance.actualWaveIndex];
		waveEnnemies = new List<EnnemiesData>();

		GameController.Instance.isEndOfWave = false;

		for (int i = 0 ; i < actualWave.ennemiesType.Count ; i++)
		{
			for (int j = 0 ; j < actualWave.ennemiesType[i].nbrToSpawn ; j++)
			{
				waveEnnemies.Add(actualWave.ennemiesType[i].data);
			}
		}

		GameController.Instance.ActualWaveDisplay(GameController.Instance.actualWaveIndex + 1);

		actualEnnemyIndex = 0;
		maxEnnemies = waveEnnemies.Count;

		LaunchWave();
	}

	private void LaunchWave()
	{
		GameController.Instance.isWaveActive = true;

		int choice = Random.Range(0, waveEnnemies.Count - 1);

		Ennemies ennemy = null;

		for (int i = 0 ; i < ennemiesPool.Count ; i++)
		{
			if (ennemiesPool[i].data == waveEnnemies[choice] && !ennemiesPool[i].gameObject.activeInHierarchy)
			{
				ennemy = ennemiesPool[i];
				break;
			}
		}

		if (ennemy == null)
		{
			GameObject newEnnemy = Instantiate(waveEnnemies[choice].prefab);
			ennemy = newEnnemy.GetComponent<Ennemies>();
			ennemy.gameObject.SetActive(false);
			ennemiesPool.Add(ennemy);
		}

		InitializeEnnemy(ennemy, waveEnnemies[choice]);

		int selectSpawn = Random.Range(0, GameController.Instance.spawns.Count);
		int selectEnd = Random.Range(0, GameController.Instance.ends.Count);
		float randomPos = Random.Range(-1f, 1f);

		ennemy.transform.position = GameController.Instance.spawns[selectSpawn].position + GameController.Instance.spawns[selectSpawn].transform.right * randomPos;
		ennemy.transform.LookAt(GameController.Instance.ends[selectEnd].position);
		ennemy.gameObject.SetActive(true);
		ennemy.SetTarget(GameController.Instance.ends[selectEnd].position + GameController.Instance.ends[selectEnd].right * (-randomPos));
		ennemy.agent.speed = ennemy.data.speed;

		waveEnnemies.RemoveAt(choice);

		actualEnnemyIndex++;
		GameController.Instance.WaveCompletion(actualEnnemyIndex, maxEnnemies);

		if (waveEnnemies.Count > 0)
			Invoke(nameof(LaunchWave), Random.Range(2f, 3.5f));
		else
		{
			GameController.Instance.isEndOfWave = true;

			// //

			GameController.Instance.isWaveActive = false;
			GameController.Instance.actualWaveIndex++;

			// //
		}
	}
}