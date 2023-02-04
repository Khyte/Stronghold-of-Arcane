using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class LoadAndSaveData
{
	private static string path;

	public static void LoadFirstTime()
	{
		path = Application.persistentDataPath;

		if (!File.Exists($"{path}/Save.json"))
			File.Create($"{path}/Save.json");

		LoadData();
	}

	public static void SaveData()
	{
		string saveJSON = JsonUtility.ToJson(DataManager.Instance.Data);

		if (File.Exists($"{path}/Save.json"))
			File.WriteAllText($"{path}/Save.json", Convert.ToBase64String(Encoding.UTF8.GetBytes(saveJSON)));
	}

	public static void LoadData()
	{
		if (File.Exists($"{path}/Save.json"))
		{
			string loadJSON = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText($"{path}/Save.json")));
			DataManager.Instance.Data = JsonUtility.FromJson<Data>(loadJSON);
		}
	}
}