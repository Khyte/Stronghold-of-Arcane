using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class, contain all saved data on devices
/// </summary>
public class Data
{
	public string version;
	public int savedWorlds;
	public float volume;

	public void InitializeData()
	{
		version = Application.version;
		savedWorlds = 0;
		volume = 1f;
	}
}

/// <summary>
/// Manager of the data of the game
/// </summary>
public class DataManager : MonoBehaviour
{
	public static DataManager Instance;

	public Data Data;

	public List<LevelsData> allLevels;
	public LevelsData actualLevel;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
			Instance = this;

		Data = LoadAndSaveData.LoadData(Data);

		if (Data == null)
		{
			Data = new Data();
			Data.InitializeData();
		}
		else if (Application.version != Data.version)
		{
			Data.version = Application.version;
			LoadAndSaveData.SaveData(Data);
		}
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0))
			CheatSaveLevel(0);
		if (Input.GetKeyDown(KeyCode.Alpha1))
			CheatSaveLevel(1);
		if (Input.GetKeyDown(KeyCode.Alpha2))
			CheatSaveLevel(2);
		if (Input.GetKeyDown(KeyCode.Alpha3))
			CheatSaveLevel(3);
		if (Input.GetKeyDown(KeyCode.Alpha4))
			CheatSaveLevel(4);
		if (Input.GetKeyDown(KeyCode.Alpha5))
			CheatSaveLevel(5);
		if (Input.GetKeyDown(KeyCode.Alpha6))
			CheatSaveLevel(6);
		if (Input.GetKeyDown(KeyCode.Alpha7))
			CheatSaveLevel(7);
		if (Input.GetKeyDown(KeyCode.Alpha8))
			CheatSaveLevel(8);
	}

	/// <summary>
	/// Only in Unity Editor : save a specify level progression
	/// </summary>
	private void CheatSaveLevel(int levelIndex)
	{
		Data.savedWorlds = levelIndex;
		LoadAndSaveData.SaveData(Data);
	}
#endif
}