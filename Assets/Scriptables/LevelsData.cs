using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelsData", order = 2)]
public class LevelsData : ScriptableObject
{
	public string levelId;
	public GameObject map;
	public List<Wave> waves;
}