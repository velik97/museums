using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
	public int index;				// -1 is trash
	public int points;

	private static Dictionary<int, int> pointsByIndex;
	
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	private void OnEnable()
	{
		if (pointsByIndex == null)
			pointsByIndex = new Dictionary<int, int>();
		
		if (!pointsByIndex.ContainsKey(index))
			pointsByIndex.Add(index, 0);

		pointsByIndex[index] += points;
	}

	private void Awake()
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;
	}

	public void PlaceInBusket(bool destroy)
	{
		if (destroy)
		{
			Destroy(gameObject);
		}
		else
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
		}
	}

	public static int PointsByIndex(int i)
	{
		if (pointsByIndex.ContainsKey(i))
			return pointsByIndex[i];

		return 0;
	}
}
