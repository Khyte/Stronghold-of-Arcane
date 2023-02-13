using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveLine : MonoBehaviour
{
	[SerializeField]
	private LineRenderer linePrefab;

	[SerializeField]
	private Material lineMat;

	private List<LineRenderer> lines;
	private List<Material> lineMats;

	private float animValue;
	private bool isAnimated;

	private void Start()
	{
		lines = new List<LineRenderer>();
		lineMats = new List<Material>();
	}

	private void Update()
	{
		if (isAnimated)
			AnimateLine();
	}

	public void CreateLine(Vector3 startPoint, Vector3 endPoint)
	{
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path);
		Vector3[] corners = path.corners;

		float distance = 0;

		for (int i = 0 ; i < corners.Length ; i++)
		{
			corners[i] = new Vector3(corners[i].x, corners[i].y + 0.2f, corners[i].z);

			if (i < corners.Length - 1)
				distance += Vector3.Distance(corners[i], corners[i + 1]);
		}

		LineRenderer line = Instantiate(linePrefab, transform);
		line.material.mainTextureScale = new Vector2(distance * 0.2f, 1);
		line.positionCount = corners.Length;
		line.numCapVertices = 5;
		line.SetPositions(corners);

		lines.Add(line);
		lineMats.Add(line.material);

		animValue = 0;
		isAnimated = true;
		line.gameObject.SetActive(true);
	}

	public void DisplayLine()
	{
		isAnimated = true;

		for (int i = 0 ; i < lines.Count ; i++)
		{
			lines[i].gameObject.SetActive(true);
		}
	}

	public void HideLine()
	{
		isAnimated = false;

		for (int i = 0 ; i < lines.Count ; i++)
		{
			lines[i].gameObject.SetActive(false);
		}
	}

	private void AnimateLine()
	{
		if (lineMats.Count > 0)
		{
			animValue -= Time.deltaTime;

			if (animValue <= -1f)
				animValue = 0;

			for (int i = 0 ; i < lineMats.Count ; i++)
			{
				lineMats[i].mainTextureOffset = new Vector2(animValue, 0);
			}
		}
	}
}