using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUpObject : MonoBehaviour
{
	public int id;				// -1 is trash
	
	private static Dictionary<Museum, List<PickUpObject>> pickUpsListByIndex; 
	
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Rigidbody rb;

	[HideInInspector]
	public ArtefactInfo artefactInfo;

	public static Dictionary<Museum, List<PickUpObject>> PickUpsListByIndex
	{
		get { return pickUpsListByIndex; }
	}

	private void Awake()
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;

		rb = GetComponent<Rigidbody>();
		
		WaveGameSetter.SubscribeOnWave(SetPickUpsDictionary, 1);
	}

	private void SetPickUpsDictionary()
	{
		artefactInfo = GameInfo.Instance.ArtefactById(id);
		
		if (pickUpsListByIndex == null)
			pickUpsListByIndex = new Dictionary<Museum, List<PickUpObject>>();
			
		if (!pickUpsListByIndex.ContainsKey(artefactInfo.museum))	
			pickUpsListByIndex.Add(artefactInfo.museum, new List<PickUpObject>());
		
		pickUpsListByIndex[artefactInfo.museum].Add(this);
	}

	public static void ResetList()
	{
		print("reset pick ups");		
		pickUpsListByIndex.Clear();
	}

	public void PlaceInBusket(bool destroy)
	{
		if (destroy)
		{
			pickUpsListByIndex[artefactInfo.museum].Remove(this);
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
