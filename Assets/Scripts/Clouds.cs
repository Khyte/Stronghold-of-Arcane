using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> clouds = new List<GameObject>();
	private List<GameObject> activeClouds = new List<GameObject>();

	private void Start()
	{
		for (int i = 0 ; i < clouds.Count ; i++)
		{
			clouds[i].SetActive(false);
		}

		DisplayCloud();
	}

	private void DisplayCloud()
	{
		if (clouds.Count <= 0)
		{
			Invoke(nameof(DisplayCloud), Random.Range(20, 30));
			return;
		}

		int cloudIndex = Random.Range(0, clouds.Count - 1);

		GameObject cloud = clouds[cloudIndex];
		clouds.RemoveAt(cloudIndex);
		cloud.SetActive(true);

		Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * Random.Range(140, 200);
		cloud.transform.position = new Vector3(randomCirclePoint.x, Random.Range(-25, -40), randomCirclePoint.y);
		cloud.transform.localScale = new Vector3(Random.Range(4f, 6f), Random.Range(4f, 6f), Random.Range(4f, 6f));
		cloud.transform.LookAt(new Vector3(Random.Range(-5, 5), cloud.transform.position.y, Random.Range(-5, 5)));

		StartCoroutine(ResetCloud(cloud));
		Invoke(nameof(DisplayCloud), Random.Range(15, 25));
	}

	private IEnumerator ResetCloud(GameObject cloud)
	{
		yield return new WaitForSeconds(180);

		cloud.SetActive(false);
		clouds.Add(cloud);
	}
}