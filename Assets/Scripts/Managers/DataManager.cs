using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
	public int savedWorlds;
	public Options savedOptions;
}

public class Options
{
	public float volume;
}

public class DataManager : MonoBehaviour
{
	public static DataManager Instance;

	public int actualLevel;

	public Data Data;

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
		FirstLoad();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0))
			SaveData(0);
		if (Input.GetKeyDown(KeyCode.Alpha1))
			SaveData(1);
		if (Input.GetKeyDown(KeyCode.Alpha2))
			SaveData(2);
		if (Input.GetKeyDown(KeyCode.Alpha3))
			SaveData(3);
		if (Input.GetKeyDown(KeyCode.Alpha4))
			SaveData(4);
		if (Input.GetKeyDown(KeyCode.Alpha5))
			SaveData(5);
		if (Input.GetKeyDown(KeyCode.Alpha6))
			SaveData(6);
		if (Input.GetKeyDown(KeyCode.Alpha7))
			SaveData(7);
		if (Input.GetKeyDown(KeyCode.Alpha8))
			SaveData(8);
	}

	private void FirstLoad()
	{
		LoadAndSaveData.LoadFirstTime();

		if (Data.savedOptions == null)
		{
			Data.savedOptions = new Options();
			Data.savedOptions.volume = 1f;
		}

		AudioListener.volume = Data.savedOptions.volume;
	}

	public void SaveData(int world = -1, float volume = -1)
	{
		Debug.LogWarning(world);

		if (world != -1)
			Data.savedWorlds = world;

		if (volume != -1)
			Data.savedOptions.volume = volume;

		LoadAndSaveData.SaveData();
	}
}