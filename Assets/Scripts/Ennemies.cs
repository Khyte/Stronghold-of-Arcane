using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ennemies : MonoBehaviour
{
	public EnnemiesData data;

	public float actualHP;

	public NavMeshAgent agent;

	public void InitializeEnnemy()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public void SetTarget(Vector3 targetPosition)
	{
		agent.speed = data.speed;
		agent.SetDestination(targetPosition);

		actualHP = data.maxHP;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "End")
		{
			GameplayController.Instance.TakeDamage();
			gameObject.SetActive(false);
		}

		if (other.tag == "Projectile")
		{
			if (float.TryParse(other.name, out float damage))
			{
				actualHP -= damage;

				if (actualHP <= 0)
					gameObject.SetActive(false);

				other.GetComponent<Collider>().enabled = false;
			}
		}
	}
}