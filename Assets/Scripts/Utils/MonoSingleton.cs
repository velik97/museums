using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class MonoSingleton <T> : MonoBehaviour where T : MonoBehaviour {

	private static T instance;

	private static object _lock = new object();

	public static T Instance {
		get {
			lock (_lock) {
				if (instance == null) {
					instance = (T) FindObjectOfType (typeof(T));

					T[] instances = (T[]) FindObjectsOfType(typeof(T));

					if (instances.Length > 1)
					{
						Debug.LogWarning("[Singleton] More than 1 singleton of type " + typeof(T).ToString () +
						                "! Deleting all despite the first one.");
						for (var i = 1; i < instances.Length; i++)
						{
							Destroy(instances[i].gameObject);						
						}
						instance = instances[0];
					} else if (instance == null) {
						Debug.LogWarning("[Singleton] There are no object of type " + typeof(T).ToString () +
						                "! Crating new insance.");
						GameObject newInstanceGameObject = new GameObject("[" + typeof(T).ToString() + "]");

						instance = newInstanceGameObject.AddComponent<T>();
					}
				}

				return instance;
			}
		}
	}

}