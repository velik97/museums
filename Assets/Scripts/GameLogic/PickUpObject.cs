using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
	public int index;				// -1 is trash
	public int points;

	private static Dictionary<int, int> pointsByIndex;
	private static Dictionary<int, int> countByIndex;
	
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Rigidbody rb;

	private void OnEnable()
	{
		if (pointsByIndex == null)
			pointsByIndex = new Dictionary<int, int>();
		
		if (countByIndex == null)
			countByIndex = new Dictionary<int, int>();
		
		if (!pointsByIndex.ContainsKey(index))
			pointsByIndex.Add(index, 0);
		
		if (!countByIndex.ContainsKey(index))
			countByIndex.Add(index, 0);

		pointsByIndex[index] += points;
		countByIndex[index]++;
	}

	private void Awake()
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;

		rb = GetComponent<Rigidbody>();
	}

	public void PlaceInBusket(bool destroy)
	{
		if (destroy)
		{
			Destroy(gameObject);
		}
		else
		{
			transform.position = initialPosition + Vector3.up * .005f;
			transform.rotation = initialRotation;

			if (rb != null)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
		}
	}

	public static int PointsByIndex(int i)
	{
		if (pointsByIndex.ContainsKey(i))
			return pointsByIndex[i];

		return 0;
	}
	
	public static int CountByIndex(int i)
	{
		if (countByIndex.ContainsKey(i))
			return countByIndex[i];

		return 0;
	}
}
