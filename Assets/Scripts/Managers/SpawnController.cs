using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	[SerializeField]
	private AudioClip spawnSound;
	private AudioSource source;

	private List<Wave> waves;
	private Wave actualWave;

	private List<EnnemiesData> waveEnnemies;
	private List<EnnemiesData> actualEnnemies;
	private List<Ennemies> ennemiesPool = new List<Ennemies>();

	private int actualEnnemyIndex;
	private int maxEnnemies;

	private void Start()
	{
		source = GetComponent<AudioSource>();
		source.clip = spawnSound;
	}

	private void InitializeEnnemy(Ennemies ennemy, EnnemiesData data)
	{
		ennemy.data = data;
		ennemy.name = data.name;
		ennemy.InitializeEnnemy();
	}

	public void StartNewWave()
	{
		PlaySpawnSound();

		GameController.Instance.waveLine.HideLine();
		GameController.Instance.battleUI.nextWaveButton.SetActive(false);
		waves = GameController.Instance.waves;

		if (GameController.Instance.actualWaveIndex >= waves.Count)
			return;

		actualWave = waves[GameController.Instance.actualWaveIndex];
		waveEnnemies = new List<EnnemiesData>();
		actualEnnemies = new List<EnnemiesData>();

		for (int i = 0 ; i < actualWave.ennemiesType.Count ; i++)
		{
			for (int j = 0 ; j < actualWave.ennemiesType[i].nbrToSpawn ; j++)
			{
				waveEnnemies.Add(actualWave.ennemiesType[i].data);
				actualEnnemies.Add(actualWave.ennemiesType[i].data);
			}
		}

		GameController.Instance.ActualWaveDisplay(GameController.Instance.actualWaveIndex + 1);

		actualEnnemyIndex = 0;
		maxEnnemies = waveEnnemies.Count;

		LaunchWave();
	}

	private void PlaySpawnSound()
	{
		float startTime = Random.Range(0, spawnSound.length - 20);
		source.time = startTime;
		GameManager.Instance.StartFadeVolume(source, true);
		source.Play();
		source.SetScheduledEndTime(AudioSettings.dspTime + 5f);

		Invoke(nameof(FadeDown), 3f);
	}

	private void FadeDown()
	{
		GameManager.Instance.StartFadeVolume(source);
	}

	private void LaunchWave()
	{
		if (GameController.Instance.life <= 0)
			return;

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

		ennemy.OnEnnemyDie += CheckVictory;

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
			Invoke(nameof(LaunchWave), Random.Range(3.5f, 5f));
	}

	private void CheckVictory(Ennemies ennemy)
	{
		ennemy.OnEnnemyDie -= CheckVictory;

		if (GameController.Instance.life <= 0)
		{
			for (int i = 0 ; i < ennemiesPool.Count ; i++)
			{
				ennemiesPool[i].gameObject.SetActive(false);
			}

			for (int i = 0 ; i < GameController.Instance.towers.Count ; i++)
			{
				GameController.Instance.towers[i].target = null;
				GameController.Instance.towers[i].availableTargets.Clear();
			}

			GameController.Instance.Lose();
		}
		else
		{
			actualEnnemies.Remove(ennemy.data);

			if (actualEnnemies.Count <= 0)
			{
				if (GameController.Instance.actualWaveIndex + 1 >= waves.Count)
					GameController.Instance.Victory();
				else
				{
					GameController.Instance.actualWaveIndex++;
					GameController.Instance.battleUI.nextWaveButton.SetActive(true);
					GameController.Instance.waveLine.DisplayLine();
				}
			}
		}
	}
}