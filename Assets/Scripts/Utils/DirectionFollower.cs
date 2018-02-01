using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirectionFollower : MonoBehaviour
{
	public Camera refCamera;
	public float speed;
	public bool onlyYAxis;
	public bool persistant;

	public Transform rootTransform;

	private void Update()
	{
		if (onlyYAxis)
		{
			float angleY = Vector3.Angle(Vector3.ProjectOnPlane(refCamera.transform.forward, Vector3.up),
				Vector3.ProjectOnPlane(rootTransform.forward, Vector3.up));

			float sign = Mathf.Sign(Vector3.Cross(rootTransform.forward, refCamera.transform.forward).y);
			float deltaAngle = angleY * sign * (persistant ? 1f : (speed * Time.deltaTime));

			rootTransform.Rotate(rootTransform.up, deltaAngle, Space.Self);
			rootTransform.position = refCamera.transform.position;
		}
		else
		{
			rootTransform.rotation = refCamera.transform.rotation;			
			rootTransform.position = refCamera.transform.position;
		}
	}

	public void Reset()
	{
		rootTransform.rotation = Quaternion.identity;
	}
}
