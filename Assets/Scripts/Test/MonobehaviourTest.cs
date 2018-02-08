using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonobehaviourTest : MonoBehaviour
{
	private void OnEnable()
	{
		print("on enable " + gameObject.name);
	}

	private void Awake()
	{
		print("awake " + gameObject.name);
	}

	private void Start()
	{
		print("start " + gameObject.name);
	}
}
