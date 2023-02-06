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

	public void InitializeEnnemy()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public void SetTarget(Vector3 targetPosition)
	{
		agent.speed = data.speed;
		agent.SetDestination(targetPosition);

		actualHP = data.maxHP + DataManager.Instance.actualLevel.levelId * data.hpModifier;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "End")
		{
			GameController.Instance.TakeDamage();
			gameObject.SetActive(false);

			OnEnnemyDie?.Invoke(this);
		}

		if (other.tag == "Projectile")
		{
			if (float.TryParse(other.name, out float damage))
			{
				actualHP -= damage;

				if (actualHP <= 0)
				{
					gameObject.SetActive(false);
					GameController.Instance.GetOrLoseMoney(data.moneyDrop);

					OnEnnemyDie?.Invoke(this);
				}

				other.GetComponent<Collider>().enabled = false;
			}
		}
	}
}