using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
	public float damage;

	public event Action<Projectiles> OnProjectileHit;

	[SerializeField]
	private GameObject model;
	[SerializeField]
	private ParticleSystem collisionParticles;

	public void InitProjectile(float damage)
	{
		this.damage = damage;
	}

	public void ResetProjectile(float damage)
	{
		if (model != null)
			model.SetActive(true);

		if (collisionParticles != null)
			collisionParticles.gameObject.SetActive(false);

		this.damage = damage;
	}

	public void Impact()
	{
		if (model != null)
			model.SetActive(false);

		if (collisionParticles != null)
		{
			collisionParticles.gameObject.SetActive(true);
			collisionParticles.Play();
		}

		Invoke(nameof(DisableProjectile), 2f);
	}

	private void DisableProjectile()
	{
		gameObject.SetActive(false);
		gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.localPosition = Vector3.zero;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Ennemy")
		{
			other.GetComponent<Ennemies>().TakeDamage(this);
			Impact();
		}
	}
}