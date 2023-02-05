using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowersData", menuName = "ScriptableObjects/TowersData", order = 1)]
public class TowersData : ScriptableObject
{
	public int id;
	public string towerName;
	public GameObject prefab;

	public float baseAttack;
	public float attackModifier;
	public float attackSpeed;
	public float range;

	public int cost;
	public int costPerUpgrade;

	public float forceOfProjectile;
	public float timeDisableProjectile;

	public bool isDotTower;
}