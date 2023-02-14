using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyingSystem : MonoBehaviour
{
	public GameObject towerPrefab;

	[SerializeField]
	private GameObject shopMenu;
	[SerializeField]
	private GameObject upgradeMenu;

	[SerializeField]
	private List<Button> upgradesBtn;
	[SerializeField]
	private List<GameObject> locksBtn;
	[SerializeField]
	private List<GameObject> okBtns;

	private Camera mainCam;
	private LayerMask selectableLayer;

	private Arcanes selectedArcane;
	private Towers selectedTower;

	private void Awake()
	{
		mainCam = Camera.main;
		selectableLayer = LayerMask.GetMask("SelectableObjects");

		shopMenu.SetActive(false);
		upgradeMenu.SetActive(false);

		for (int i = 1 ; i < upgradesBtn.Count ; i++)
		{
			upgradesBtn[i].enabled = false;
		}
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
				Arcanes arcane = hit.transform.GetComponent<Arcanes>();

				if (selectedArcane != null && selectedArcane == arcane)
					return;

				selectedArcane = arcane;

				if (selectedTower != null)
					selectedTower.range.SetActive(false);

				selectedTower = null;

				if (arcane.actualTower != null)
				{
					selectedTower = arcane.actualTower;
					selectedTower.range.SetActive(true);

					List<int> costs = new List<int>();

					for (int i = 1 ; i < 4 ; i++)
					{
						int cost = selectedTower.data.costPerUpgrade * i;
						costs.Add(cost);
					}

					GameController.Instance.battleUI.DisplayUpgradeCost(costs, selectedTower.data.cost);
					DisplayUpgrade(selectedTower.actualUpgrade);

					shopMenu.SetActive(false);
					upgradeMenu.SetActive(true);
				}
				else
				{
					shopMenu.SetActive(true);
					upgradeMenu.SetActive(false);
				}
			}
		}
		else
		{
			shopMenu.SetActive(false);
			upgradeMenu.SetActive(false);

			selectedArcane = null;

			if (selectedTower != null)
			{
				selectedTower.range.SetActive(false);
				selectedTower = null;
			}
		}
	}

	private void DisplayUpgrade(int upgradeIndex)
	{
		switch (upgradeIndex)
		{
			case 1:
				upgradesBtn[0].enabled = false;
				upgradesBtn[1].enabled = true;
				upgradesBtn[2].enabled = false;

				locksBtn[0].SetActive(false);
				locksBtn[1].SetActive(true);

				okBtns[0].SetActive(true);
				okBtns[1].SetActive(false);
				okBtns[2].SetActive(false);
				break;
			case 2:
				upgradesBtn[0].enabled = false;
				upgradesBtn[1].enabled = false;
				upgradesBtn[2].enabled = true;

				locksBtn[0].SetActive(false);
				locksBtn[1].SetActive(false);

				okBtns[0].SetActive(true);
				okBtns[1].SetActive(true);
				okBtns[2].SetActive(false);
				break;
			case 3:
				upgradesBtn[0].enabled = false;
				upgradesBtn[1].enabled = false;
				upgradesBtn[2].enabled = false;

				locksBtn[0].SetActive(false);
				locksBtn[1].SetActive(false);

				okBtns[0].SetActive(true);
				okBtns[1].SetActive(true);
				okBtns[2].SetActive(true);
				break;
			default:
				upgradesBtn[0].enabled = true;
				upgradesBtn[1].enabled = false;
				upgradesBtn[2].enabled = false;

				locksBtn[0].SetActive(true);
				locksBtn[1].SetActive(true);

				okBtns[0].SetActive(false);
				okBtns[1].SetActive(false);
				okBtns[2].SetActive(false);
				break;
		}
	}

	public void BuyTower(TowersData tower)
	{
		if (tower.cost > GameController.Instance.money)
			return;

		GameController.Instance.GetOrLoseMoney(-tower.cost);

		GameObject newTower = Instantiate(tower.prefab, selectedArcane.transform.position - selectedArcane.transform.up * 0.5f, Quaternion.identity, selectedArcane.transform);
		Towers compTower = newTower.GetComponent<Towers>();
		selectedTower = compTower;
		selectedTower.range.SetActive(true);
		selectedArcane.actualTower = compTower;

		List<int> costs = new List<int>();

		for (int i = 1 ; i < 4 ; i++)
		{
			int cost = selectedTower.data.costPerUpgrade * i;
			costs.Add(cost);
		}

		GameController.Instance.battleUI.DisplayUpgradeCost(costs, selectedTower.data.cost);
		DisplayUpgrade(selectedTower.actualUpgrade);

		shopMenu.SetActive(false);
		upgradeMenu.SetActive(true);
		compTower.range.SetActive(true);

		GameController.Instance.towers.Add(compTower);
	}

	public void SellTower()
	{
		GameController.Instance.GetOrLoseMoney(selectedTower.data.cost);
		GameController.Instance.towers.Remove(selectedTower);
		selectedArcane.actualTower = null;

		Destroy(selectedTower.gameObject);

		shopMenu.SetActive(true);
		upgradeMenu.SetActive(false);
	}

	public void UpgradeTower(int upgrade)
	{
		int cost = selectedTower.data.costPerUpgrade * upgrade;

		if (cost > GameController.Instance.money)
			return;

		GameController.Instance.GetOrLoseMoney(-cost);

		selectedTower.actualDamage = selectedTower.data.baseAttack + (selectedTower.data.attackModifier * upgrade);
		selectedTower.actualUpgrade = upgrade;

		selectedTower.models.GetChild(upgrade - 1).gameObject.SetActive(false);
		selectedTower.models.GetChild(upgrade).gameObject.SetActive(true);

		DisplayUpgrade(upgrade);
	}
}