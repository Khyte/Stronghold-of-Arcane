using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyingSystem : MonoBehaviour
{
	public GameObject towerPrefab;

	[SerializeField]
	private GameObject shopMenu;
	[SerializeField]
	private GameObject upgradeMenu;

	private Camera mainCam;
	private LayerMask selectableLayer;
	private LayerMask uiLayer;

	private GameObject selectedArcane;
	private GameObject selectedTower;

	private void Awake()
	{
		mainCam = Camera.main;
		selectableLayer = LayerMask.GetMask("SelectableObjects");
		uiLayer = LayerMask.GetMask("UI");

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

	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0;
	}

	private void CheckForOpeningMenu()
	{
		if (mainCam == null)
			return;

		if (IsPointerOverUIObject())
			return;

		Ray ray;

#if UNITY_EDITOR
		ray = mainCam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
#else
		Touch touch = Input.GetTouch(0);
		ray = mainCam.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));
#endif

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayer))
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
		else
		{
			shopMenu.SetActive(false);
			upgradeMenu.SetActive(false);
		}
	}

	public void BuyTower(TowersData tower)
	{
		if (tower.cost > GameController.Instance.money)
			return;

		GameController.Instance.GetOrLoseMoney(-tower.cost);

		GameObject newTower = Instantiate(tower.prefab, selectedArcane.transform.position - selectedArcane.transform.up * 0.5f, Quaternion.identity, selectedArcane.transform);
		selectedArcane.GetComponent<Collider>().enabled = false;
		shopMenu.SetActive(false);

		GameController.Instance.towers.Add(newTower.GetComponent<Towers>());
	}

	public void SellTower(Towers tower)
	{
		GameController.Instance.GetOrLoseMoney(tower.data.cost);
		GameController.Instance.towers.Remove(tower);

		Destroy(tower.gameObject);
		selectedArcane.GetComponent<Collider>().enabled = true;
		shopMenu.SetActive(false);
	}

	public void UpgradeTower(int upgrade)
	{
		Towers tower = selectedTower.GetComponent<Towers>();

		if (tower.data.costPerUpgrade > GameController.Instance.money)
			return;

		GameController.Instance.GetOrLoseMoney(-tower.data.costPerUpgrade);

		tower.actualDamage = tower.data.baseAttack + (tower.data.attackModifier * upgrade);
		tower.actualUpgrade = upgrade;
		upgradeMenu.SetActive(false);
		tower.range.gameObject.SetActive(false);
	}
}