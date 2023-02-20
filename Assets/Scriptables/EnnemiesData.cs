using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnnemiesData", menuName = "ScriptableObjects/EnnemiesData", order = 0)]
public class EnnemiesData : ScriptableObject
{
	public int id;
	public string ennemyName;
	public GameObject monsterPrefab;
	public GameObject model;

	public float baseHP;
	public float hpModifier;
	public float maxHP;
	public float actualHP;
	public float speed;
	public float width;

	public int moneyDrop;
}