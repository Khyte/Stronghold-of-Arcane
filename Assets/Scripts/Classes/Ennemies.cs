using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ennemies : MonoBehaviour
{
	public EnnemiesData data;

	public event Action<Ennemies> OnEnnemyDie;

	public float actualHP;

	[SerializeField]
	private GameObject model;
	[SerializeField]
	private BoxCollider boxCollider;
	[SerializeField]
	private GameObject deathParticles;

	private NavMeshAgent agent;

	public void InitializeEnnemy(EnnemiesData ennemyData)
	{
		data = ennemyData;
		name = data.name;
		actualHP = data.maxHP + DataManager.Instance.actualLevel.levelId * data.hpModifier;
		boxCollider.size *= 2 * data.width;

		agent = GetComponent<NavMeshAgent>();
		agent.speed = data.speed;
		agent.radius = data.width;

		if (model.transform.childCount == 0)
			Instantiate(data.model, model.transform);

		deathParticles.SetActive(false);
		model.SetActive(true);
	}

	public void SetTarget(Vector3 targetPosition)
	{
		agent.enabled = true;
		agent.speed = data.speed;
		agent.SetDestination(targetPosition);
	}

	public float GetDistanceFromEnd()
	{
		float distance = 0;
		Vector3[] points = agent.path.corners;

		for (int k = 0 ; k < points.Length - 1 ; k++)
		{
			distance += Vector3.Distance(points[k], points[k + 1]);
		}

		return distance;
	}

	private void DisableEnnemy()
	{
		gameObject.SetActive(false);
	}

	public void TakeDamage(Projectiles projectile)
	{
		if (actualHP <= 0)
			return;

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
			GameController.Instance.LoseLife(transform.position);
			gameObject.SetActive(false);
			OnEnnemyDie?.Invoke(this);
		}
	}
}