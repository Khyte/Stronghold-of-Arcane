using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour
{
	public TowersData data;

	[SerializeField]
	private Projectiles projectilePrefab;
	[SerializeField]
	private Transform spawnProjectile;
	[SerializeField]
	private GameObject dotParticles;

	public Transform models;

	public GameObject range;

	public Ennemies target;
	public List<Ennemies> availableTargets = new List<Ennemies>();

	public List<Projectiles> poolOfProjectiles = new List<Projectiles>();

	public float actualDamage;
	public int actualUpgrade = 0;

	private bool isShooting;

	private void Start()
	{
		if (dotParticles != null)
			dotParticles.SetActive(false);

		InitializeTower();
	}

	private void InitializeTower()
	{
		GetComponent<CapsuleCollider>().radius = data.range;
		actualDamage = data.baseAttack;
		range.transform.localScale *= data.range * 0.2f;
		range.SetActive(false);
	}

	private Projectiles CreateNewProjectile()
	{
		Projectiles projectile = Instantiate(projectilePrefab, transform);
		projectile.InitProjectile(actualDamage);
		projectile.gameObject.SetActive(false);
		poolOfProjectiles.Add(projectile);

		return projectile;
	}

	private void Fire()
	{
		isShooting = true;
		Shoot();

		if (dotParticles != null)
			dotParticles.SetActive(true);
	}

	private void Shoot()
	{
		Projectiles projectile = null;

		for (int i = 0 ; i < poolOfProjectiles.Count ; i++)
		{
			if (!poolOfProjectiles[i].gameObject.activeInHierarchy)
			{
				projectile = poolOfProjectiles[i];
				break;
			}
		}

		if (projectile == null)
			projectile = CreateNewProjectile();

		spawnProjectile.transform.LookAt(target.transform.position, Vector3.forward);

		projectile.transform.position = spawnProjectile.position;
		projectile.transform.rotation = spawnProjectile.transform.rotation;
		projectile.ResetProjectile(actualDamage);
		projectile.gameObject.SetActive(true);
		projectile.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, data.forceOfProjectile));

		StartCoroutine(NextProjectile());
	}

	IEnumerator NextProjectile()
	{
		yield return new WaitForSeconds(data.attackSpeed);

		if (target != null && target.actualHP <= 0)
			availableTargets.Remove(target);

		target = GetMostDangerousTarget();

		if (target == null)
		{
			isShooting = false;

			if (dotParticles != null)
				dotParticles.SetActive(false);
		}
		else
			Shoot();
	}

	private Ennemies GetMostDangerousTarget()
	{
		if (availableTargets.Count == 0)
			return null;

		Ennemies potentialTarget = null;
		float distanceToEnd = Mathf.Infinity;

		for (int i = availableTargets.Count - 1 ; i >= 0 ; i--)
		{
			if (availableTargets[i].actualHP <= 0)
				availableTargets.RemoveAt(i);
			else
			{
				for (int j = 0 ; j < GameController.Instance.ends.Count ; j++)
				{
					float distance = 0;
					Vector3[] points = availableTargets[i].agent.path.corners;

					for (int k = 0 ; k < points.Length - 1 ; k++)
					{
						distance += Vector3.Distance(points[k], points[k + 1]);
					}

					if (distanceToEnd > distance)
					{
						distanceToEnd = distance;
						potentialTarget = availableTargets[i];
					}
				}
			}
		}

		if (potentialTarget != null)
			return potentialTarget;
		else
			return null;
	}

	private void Dot()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Ennemy")
		{
			Ennemies newTarget = other.GetComponent<Ennemies>();

			if (newTarget.actualHP <= 0)
				return;

			availableTargets.Add(newTarget);

			if (availableTargets.Count > 1)
				target = GetMostDangerousTarget();
			else if (availableTargets.Count == 1)
				target = newTarget;

			if (!isShooting && target != null)
				Fire();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Ennemy")
		{
			Ennemies removedTarget = other.GetComponent<Ennemies>();
			availableTargets.Remove(removedTarget);

			target = GetMostDangerousTarget();

			if (!isShooting && target != null)
				Fire();
		}
	}
}