using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
	public TextMeshProUGUI lifeText;
	public TextMeshProUGUI moneyText;

	public GameObject nextWaveButton;
	public GameObject winMenu;
	public GameObject loseMenu;

	public TextMeshProUGUI waveText;
	public Image waveCompletion;

	[SerializeField]
	private List<TextMeshProUGUI> shopCosts;
	[SerializeField]
	private List<TextMeshProUGUI> upgradeCosts;
	[SerializeField]
	private TextMeshProUGUI sellingPrice;

	public void InitUI(int money)
	{
		lifeText.text = "5";
		moneyText.text = money.ToString();
		waveText.text = "";
		waveCompletion.fillAmount = 0;

		for (int i = 0 ; i < shopCosts.Count ; i++)
		{
			shopCosts[i].text = $"{GameController.Instance.towersData[i].cost}C";
		}
	}

	public void DisplayUpgradeCost(List<int> costs, int sellingPrice)
	{
		for (int i = 0 ; i < upgradeCosts.Count ; i++)
		{
			upgradeCosts[i].text = $"{costs[i]}C";
		}

		this.sellingPrice.text = $"+{sellingPrice}C";
	}
}