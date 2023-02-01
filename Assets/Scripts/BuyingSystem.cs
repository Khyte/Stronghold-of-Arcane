using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingSystem : MonoBehaviour
{
    public GameObject towerPrefab;

	[SerializeField]
	private GameObject shopMenu;
	[SerializeField]
	private GameObject upgradeMenu;

	private Camera mainCam;
	private LayerMask layerMask;

	private GameObject selectedArcane;
	private GameObject selectedTower;

	private void Awake()
	{
		mainCam = Camera.main;
		layerMask = LayerMask.GetMask("SelectableObjects");

		shopMenu.SetActive(false);
		upgradeMenu.SetActive(false);
	}

	private void Update()
	{
		if (Input.touchCount > 0)
			CheckForOpeningMenu();

#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
			CheckForOpeningMenu();
#endif
	}

	private void CheckForOpeningMenu()
	{
		if (mainCam == null)
			return;

		Ray ray;

#if UNITY_EDITOR
		ray = mainCam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
#else
		Touch touch = Input.GetTouch(0);
		ray = mainCam.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));
#endif

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
		{
			if (hit.transform.tag == "Arcane")
			{
				selectedArcane = hit.transform.gameObject;
				shopMenu.transform.position = mainCam.WorldToScreenPoint(hit.transform.position);
				shopMenu.SetActive(true);
			}
			else if (hit.transform.tag == "Tower")
			{
				selectedTower = hit.transform.parent.gameObject;
				selectedTower.GetComponent<Towers>().range.gameObject.SetActive(true);
				int upgrade = selectedTower.GetComponent<Towers>().actualUpgrade + 1;

				if (upgrade > 3)
					return;

				int childIndex = 0;

				foreach (Transform child in upgradeMenu.transform)
				{
					if (childIndex == upgrade - 1)
						child.gameObject.SetActive(true);
					else
						child.gameObject.SetActive(false);

					childIndex++;
				}
				
				upgradeMenu.transform.position = mainCam.WorldToScreenPoint(hit.transform.position);
				upgradeMenu.SetActive(true);
			}
		}
	}

	public void BuyTower(GameObject towerPrefab)
	{
		Instantiate(towerPrefab, selectedArcane.transform.position, Quaternion.identity, selectedArcane.transform);
		selectedArcane.GetComponent<Collider>().enabled = false;
		shopMenu.SetActive(false);
	}

	public void UpgradeTower(int upgrade)
	{
		Towers tower = selectedTower.GetComponent<Towers>();
		tower.actualDamage = tower.data.baseAttack + (tower.data.attackModifier * upgrade);
		tower.actualUpgrade = upgrade;
		upgradeMenu.SetActive(false);
		tower.range.gameObject.SetActive(true);
	}
}