using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	private List<Wave> waves;
	private Wave actualWave;

	private List<EnnemiesData> waveEnnemies;
	public List<Ennemies> ennemiesPool = new List<Ennemies>();

	private void InitializeEnnemy(Ennemies ennemy, EnnemiesData data)
	{
		ennemy.name = data.name;
		ennemy.data = data;
		ennemy.InitializeEnnemy();
	}

	public void SpawnEnnemies()
	{
		waves = GameController.Instance.waves;

		for (int i = 0 ; i < waves[0].ennemiesType.Count ; i++)
		{
			for (int j = 0 ; j < waves[0].ennemiesType[i].nbrToSpawn ; j++)
			{
				GameObject ennemy = Instantiate(waves[0].ennemiesType[i].data.prefab, GameController.Instance.spawn.position, Quaternion.identity, null);

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
			ennemiesPool.Add(ennemy);
		}

		InitializeEnnemy(ennemy, waveEnnemies[choice]);

		float randomPos = Random.Range(-1f, 1f);
		ennemy.transform.position = GameController.Instance.spawn.position + GameController.Instance.spawn.transform.right * randomPos;
		ennemy.transform.LookAt(GameController.Instance.end.position);
		ennemy.gameObject.SetActive(true);
		ennemy.SetTarget(GameController.Instance.end.position + GameController.Instance.end.right * (-randomPos));
		ennemy.agent.speed = ennemy.data.speed;

		waveEnnemies.RemoveAt(choice);

		if (waveEnnemies.Count > 0)
			Invoke(nameof(LaunchWave), Random.Range(1.5f, 3f));
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