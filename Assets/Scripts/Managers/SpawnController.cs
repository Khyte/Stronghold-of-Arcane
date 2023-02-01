using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	[SerializeField]
	private Transform baseTarget;
	[SerializeField]
	private Transform spawn;

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
		waves = GameplayController.Instance.waves;

		for (int i = 0 ; i < waves[0].ennemiesType.Count ; i++)
		{
			for (int j = 0 ; j < waves[0].ennemiesType[i].nbrToSpawn ; j++)
			{
				GameObject ennemy = Instantiate(waves[0].ennemiesType[i].data.prefab, spawn.transform.position, Quaternion.identity, null);

				Ennemies ennemyComp = ennemy.GetComponent<Ennemies>();
				InitializeEnnemy(ennemyComp, waves[0].ennemiesType[i].data);

				ennemy.SetActive(false);
				ennemiesPool.Add(ennemyComp);
			}
		}
	}

	public void StartNewWave()
	{
		if (GameplayController.Instance.actualWaveIndex >= waves.Count)
			return;

		actualWave = waves[GameplayController.Instance.actualWaveIndex];
		waveEnnemies = new List<EnnemiesData>();

		GameplayController.Instance.isEndOfWave = false;

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
		GameplayController.Instance.isWaveActive = true;

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
		ennemy.transform.position = spawn.position + spawn.transform.right * randomPos;
		ennemy.transform.rotation = spawn.rotation;
		ennemy.gameObject.SetActive(true);
		ennemy.SetTarget(GameplayController.Instance.endPosition.position + GameplayController.Instance.endPosition.right * (-randomPos));
		ennemy.agent.speed = ennemy.data.speed;

		waveEnnemies.RemoveAt(choice);

		if (waveEnnemies.Count > 0)
			Invoke(nameof(LaunchWave), Random.Range(1.5f, 3f));
		else
		{
			GameplayController.Instance.isEndOfWave = true;

			// //

			GameplayController.Instance.isWaveActive = false;
			GameplayController.Instance.actualWaveIndex++;

			// //
		}
	}
}