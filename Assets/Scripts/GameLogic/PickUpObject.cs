using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
	public int index;				// -1 is trash
	public int points;
	public string pickUpName;

	private static Dictionary<int, List<PickUpObject>> pickUpsListByIndex; 
	
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Rigidbody rb;

	public static Dictionary<int, List<PickUpObject>> PickUpsListByIndex
	{
		get { return pickUpsListByIndex; }
	}
	
	private void OnEnable()
	{
		if (pickUpsListByIndex == null)
			pickUpsListByIndex = new Dictionary<int, List<PickUpObject>>();
			
		if (!pickUpsListByIndex.ContainsKey(index))	
			pickUpsListByIndex.Add(index, new List<PickUpObject>());
		
		pickUpsListByIndex[index].Add(this);
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
			pickUpsListByIndex[index].Remove(this);
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
		
}
