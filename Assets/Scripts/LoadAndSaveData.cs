using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Load and save data to a json file with encoding
/// </summary>
public static class LoadAndSaveData
{
	private static string path = Application.persistentDataPath;

	/// <summary>
	/// Save a class to a json file
	/// </summary>
	/// <typeparam name="T">Type of class to save</typeparam>
	/// <param name="data">The class to save</param>
	public static void SaveData<T>(T data)
	{
		string saveJSON = JsonUtility.ToJson(data);
		File.WriteAllText($"{path}/Save.json", Convert.ToBase64String(Encoding.UTF8.GetBytes(saveJSON)));
	}

	/// <summary>
	/// Create a new saved file if it not exist, or load data to a class
	/// </summary>
	/// <typeparam name="T">Type of class to load</typeparam>
	/// <param name="data">The class to load</param>
	/// <returns></returns>
	public static T LoadData<T>(T data)
	{
		if (!File.Exists($"{path}/Save.json"))
		{
			using (FileStream file = File.Open($"{path}/Save.json", FileMode.OpenOrCreate))
			{
				string saveJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonUtility.ToJson(data)));
				byte[] info = Encoding.UTF8.GetBytes(saveJSON);
				file.Write(info, 0, info.Length);

				return data;
			}
		}
		else
		{
			string loadJSON = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText($"{path}/Save.json")));
			data = JsonUtility.FromJson<T>(loadJSON);

			return data;
		}
	}
}