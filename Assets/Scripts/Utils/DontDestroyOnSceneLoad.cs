using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnSceneLoad : MonoBehaviour
{
	private static List<GameObject> notDestroyableObjects;
	
	private void Awake()
	{
		DontDestroyOnLoad(this);
		
		if (notDestroyableObjects == null)
			notDestroyableObjects = new List<GameObject>();
		
		notDestroyableObjects.Add(gameObject);
	}

	public static void DestroyAll()
	{
		if (notDestroyableObjects == null || notDestroyableObjects.Count == 0)
			return;
		
		for (var i = 0; i < notDestroyableObjects.Count; i++)
		{
			GameObject.Destroy(notDestroyableObjects[i]);
		}
		
		notDestroyableObjects.Clear();
	}
}
