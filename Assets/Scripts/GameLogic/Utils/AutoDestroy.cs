using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	public float time;

	private void Start()
	{
		StartCoroutine(DestroyInTime());
	}

	private IEnumerator DestroyInTime()
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}
