using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPickUpObject : MonoBehaviour {

	public int index;				// -1 is trash

	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Rigidbody rb;

	[HideInInspector]
	public ArtefactInfo artefactInfo;


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
}
