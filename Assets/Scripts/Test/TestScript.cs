using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
	private void Awake()
	{
		TestMachineName();
	}

	private void TestMachineName()
	{
		print(System.Environment.MachineName);
	}

	private void TestExcel()
	{
		
	}
}
