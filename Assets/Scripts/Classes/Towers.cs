using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towers : MonoBehaviour
{
	public TowersData data;

	[SerializeField]
	private GameObject projectilePrefab;
	[SerializeField]
	private Transform spawnProjectile;

	public Projector range;

	public Ennemies target;
	public List<Ennemies> availableTargets = new List<Ennemies>();

	public List<GameObject> poolOfProjectiles = new List<GameObject>();

	public float actualDamage;
	public int actualUpgrade = 0;

	private bool isShooting;

	private void Start()
	{
		InitializeTower();

		if (!data.isDotTower)
			CreateNewProjectile();
		/*else
			Do smth();*/
	}

	private void InitializeTower()
	{
		GetComponent<CapsuleCollider>().radius = data.range;
		actualDamage = data.baseAttack;
		range.fieldOfView = data.range * 30;
		range.gameObject.SetActive(false);
	}

	private GameObject CreateNewProjectile()
	{
		GameObject projectile = Instantiate(projectilePrefab, transform);
		projectile.SetActive(false);
		poolOfProjectiles.Add(projectile);

		return projectile;
	}

	private void Fire()
	{
		isShooting = true;

		if (data.isDotTower)
			Dot();
		else
			Shoot();
	}

	private void Shoot()
	{
		GameObject projectile = null;

		for (int i = 0 ; i < poolOfProjectiles.Count ; i++)
		{
			if (!poolOfProjectiles[i].activeInHierarchy)
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
		projectile.name = actualDamage.ToString();
		projectile.SetActive(true);

		projectile.GetComponent<Collider>().enabled = true;
		projectile.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, data.forceOfProjectile));

		StartCoroutine(NextProjectile(projectile, data.timeDisableProjectile));
	}

	IEnumerator NextProjectile(GameObject projectile, float waitTime)
	{
		yield return new WaitForSeconds(data.attackSpeed);

		if (target != null && target.actualHP <= 0)
			availableTargets.Remove(target);

		target = GetMostDangerousTarget();

		if (target == null)
			isShooting = false;
		else
			Shoot();

		if (waitTime > data.attackSpeed)
			yield return new WaitForSeconds(waitTime - data.attackSpeed);
		else
			yield return new WaitForEndOfFrame();

		if (projectile != null)
			ResetProjectile(projectile);
	}

	private void ResetProjectile(GameObject projectile)
	{
		projectile.SetActive(false);
		projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
		projectile.transform.position = spawnProjectile.position;
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
					float distance = Vector3.Distance(availableTargets[i].transform.position, GameController.Instance.ends[j].position);

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