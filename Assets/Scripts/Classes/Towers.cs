using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour
{
	public TowersData data;

	public float actualDamage;
	public int actualUpgrade;

	[SerializeField]
	private Transform modelsParent;
	[SerializeField]
	private Transform spawnProjectile;
	[SerializeField]
	private GameObject range;
	[SerializeField]
	private CapsuleCollider capsCollider;

	private Ennemies actualTarget;
	private List<Ennemies> availableTargets = new List<Ennemies>();
	private List<Projectiles> poolOfProjectiles = new List<Projectiles>();

	private List<GameObject> listOfModels;
	private GameObject dotParticles;
	private bool isShooting;

	private MaterialPropertyBlock mpb;

	private void Start()
	{
		if (dotParticles != null)
			dotParticles.SetActive(false);
	}

	public void InitializeTower()
	{
		listOfModels = new List<GameObject>();

		mpb = new MaterialPropertyBlock();

		for (int i = 0 ; i < data.towerModels.Count ; i++)
		{
			GameObject tower = Instantiate(data.towerModels[i], modelsParent);
			listOfModels.Add(tower);

			MeshRenderer mesh = tower.GetComponentInChildren<MeshRenderer>();
			mesh.GetPropertyBlock(mpb);
			mpb.SetColor("_EmissionColor", data.emissiveColor);
			mesh.SetPropertyBlock(mpb);

			if (i == 0)
				tower.SetActive(true);
			else
				tower.SetActive(false);
		}

		if (data.dotParticles != null)
			dotParticles = Instantiate(data.dotParticles, spawnProjectile);

		capsCollider.radius = data.range;
		actualDamage = data.baseAttack;
		range.transform.localScale *= data.range * 0.2f;
		range.SetActive(false);
	}

	public void DisplayAttackRange(bool activateRange)
	{
		range.SetActive(activateRange);
	}

	public void DisplayTowerModels(int towerIndex)
	{
		if (towerIndex >= listOfModels.Count)
			return;

		for (int i = 0 ; i < listOfModels.Count ; i++)
		{
			if (towerIndex == i)
				listOfModels[i].SetActive(true);
			else
				listOfModels[i].SetActive(false);
		}
	}

	public void ResetTargets()
	{
		isShooting = false;
		actualTarget = null;
		availableTargets.Clear();

		if (dotParticles != null)
			dotParticles.SetActive(false);
	}

	private Projectiles CreateNewProjectile()
	{
		Projectiles projectile = Instantiate(data.projectile, transform);
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

		spawnProjectile.transform.LookAt(actualTarget.transform.position, Vector3.forward);

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

		if (actualTarget != null && actualTarget.actualHP <= 0)
			availableTargets.Remove(actualTarget);

		actualTarget = GetMostDangerousTarget();

		if (actualTarget == null)
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
				float distance = availableTargets[i].GetDistanceFromEnd();

				if (distanceToEnd > distance)
				{
					distanceToEnd = distance;
					potentialTarget = availableTargets[i];
				}
			}
		}

		if (potentialTarget != null)
			return potentialTarget;
		else
			return null;
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
				actualTarget = GetMostDangerousTarget();
			else if (availableTargets.Count == 1)
				actualTarget = newTarget;

			if (!isShooting && actualTarget != null)
				Fire();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Ennemy")
		{
			Ennemies removedTarget = other.GetComponent<Ennemies>();
			availableTargets.Remove(removedTarget);

			actualTarget = GetMostDangerousTarget();

			if (!isShooting && actualTarget != null)
				Fire();
		}
	}
}