using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ennemies : MonoBehaviour
{
	public EnnemiesData data;
	public NavMeshAgent agent;

	public event Action<Ennemies> OnEnnemyDie;

	public float actualHP;

	[SerializeField]
	private GameObject model;
	[SerializeField]
	private GameObject deathParticles;

	public void InitializeEnnemy()
	{
		agent = GetComponent<NavMeshAgent>();
		deathParticles.SetActive(false);
		model.SetActive(true);
	}

	public void SetTarget(Vector3 targetPosition)
	{
		agent.enabled = true;
		agent.speed = data.speed;
		agent.SetDestination(targetPosition);

		actualHP = data.maxHP + DataManager.Instance.actualLevel.levelId * data.hpModifier;
	}

	private void DisableEnnemy()
	{
		gameObject.SetActive(false);
	}

	public void TakeDamage(Projectiles projectile)
	{
		actualHP -= projectile.damage;

		if (actualHP <= 0)
		{
			deathParticles.SetActive(true);
			model.SetActive(false);
			agent.enabled = false;
			GameController.Instance.GetOrLoseMoney(data.moneyDrop);

			OnEnnemyDie?.Invoke(this);
			Invoke(nameof(DisableEnnemy), 2f);

		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "End")
		{
			GameController.Instance.TakeDamage();
			gameObject.SetActive(false);
			OnEnnemyDie?.Invoke(this);
		}
	}
}