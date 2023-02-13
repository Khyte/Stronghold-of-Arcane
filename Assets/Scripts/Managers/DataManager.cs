using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
	public string version;
	public int savedWorlds;
	public float volume = 1f;
}

public class DataManager : MonoBehaviour
{
	public static DataManager Instance;

	public List<LevelsData> allLevels;
	public LevelsData actualLevel;

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

		LoadAndSaveData.LoadFirstTime();
	}

	private void Start()
	{
		AudioListener.volume = Data.volume;
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

	public void SaveData(int world = -1, float volume = -1)
	{
		if (world != -1)
			Data.savedWorlds = world;

		if (volume >= 0 && volume <= 1)
			Data.volume = volume;

		LoadAndSaveData.SaveData();
	}

	public float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
	{
		var fromAbs = from - fromMin;
		var fromMaxAbs = fromMax - fromMin;

		var normal = fromAbs / fromMaxAbs;

		var toMaxAbs = toMax - toMin;
		var toAbs = toMaxAbs * normal;

		var to = toAbs + toMin;

		return to;
	}
}