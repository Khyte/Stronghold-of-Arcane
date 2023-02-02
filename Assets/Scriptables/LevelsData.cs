using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelsData", order = 2)]
public class LevelsData : ScriptableObject
{
	public string levelId;
	public GameObject map;
	public NavMeshData navMesh;
	public List<Wave> waves;
	public Vector3 spawn;
	public Vector3 end;
}