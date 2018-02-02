using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUser : MonoBehaviour
{
	public Camera refCamera;

	private void Awake()
	{
		refCamera = Camera.main;
	}

	private void Update()
	{
		if (refCamera == null)
			return;
		
		float angleY = Vector3.Angle(Vector3.ProjectOnPlane(refCamera.transform.forward, Vector3.up),
			Vector3.ProjectOnPlane(transform.forward, Vector3.up));

		float sign = Mathf.Sign(Vector3.Cross(transform.forward, refCamera.transform.forward).y);
		angleY *= sign;

		transform.Rotate(transform.up, angleY, Space.Self);
	}
}
