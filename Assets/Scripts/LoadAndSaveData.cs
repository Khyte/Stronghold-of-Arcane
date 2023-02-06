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
		{
			using (FileStream file = File.Open($"{path}/Save.json", FileMode.OpenOrCreate))
			{
				DataManager.Instance.Data = new Data();
				DataManager.Instance.Data.savedWorlds = 0;
				DataManager.Instance.Data.volume = 1;

				string saveJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonUtility.ToJson(DataManager.Instance.Data)));
				byte[] info = Encoding.UTF8.GetBytes(saveJSON);
				file.Write(info, 0, info.Length);
			}

			DataManager.Instance.Data = new Data();
			DataManager.Instance.Data.savedWorlds = 0;
			DataManager.Instance.Data.volume = 1;

			SaveData();
		}
		else
			LoadData();
	}

	public static void SaveData()
	{
		string saveJSON = JsonUtility.ToJson(DataManager.Instance.Data);
		File.WriteAllText($"{path}/Save.json", Convert.ToBase64String(Encoding.UTF8.GetBytes(saveJSON)));
	}

	public static void LoadData()
	{
		string loadJSON = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText($"{path}/Save.json")));
		DataManager.Instance.Data = JsonUtility.FromJson<Data>(loadJSON);
	}
}