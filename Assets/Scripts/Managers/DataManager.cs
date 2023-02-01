using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
	public string actualWorld;
}

public class DataManager : MonoBehaviour
{
	public static DataManager Instance;

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

		LoadAndSaveData.Instance.LoadData();
	}
}