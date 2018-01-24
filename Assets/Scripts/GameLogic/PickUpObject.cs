using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
	public int index;				// -1 is trash
	public int points;

	private static Dictionary<int, int> pointsByIndex;

	private void OnEnable()
	{
		if (pointsByIndex == null)
			pointsByIndex = new Dictionary<int, int>();
		
		if (!pointsByIndex.ContainsKey(index))
			pointsByIndex.Add(index, 0);

		pointsByIndex[index] += points;
	}

	public static int PointsByIndex(int i)
	{
		if (pointsByIndex.ContainsKey(i))
			return pointsByIndex[i];

		return 0;
	}
}
