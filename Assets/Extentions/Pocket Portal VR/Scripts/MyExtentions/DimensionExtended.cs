using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Object = UnityEngine.Object;

public class DimensionExtended : Dimension
{
	public int index;
	
	public static DimensionExtended initial;
	private static DimensionExtended current;
	public static List<DimensionExtended> allDimensions;

	private static List<Type> interactionComponentTypes = new List<Type>()
	{
		typeof(VRTK_InteractableObject),
		typeof(VRTK_Button)		
	};
	
	public static DimensionExtended Current
	{
		get { return current; }
		set
		{
			Debug.Log("[Portal] Current Dimension is \"" + value.gameObject.name + "\"");
			current.SwitchOffInteractions();
			current = value;
			current.SwitchOnInteractions();
		}
	}

	private void Start()
	{
		if (initialWorld)
		{
			current = this;
			initial = this;
			SwitchOnInteractions();
		}
		else
		{
			SwitchOffInteractions();
		}

		if (allDimensions == null)
			allDimensions = new List<DimensionExtended>();
		
		allDimensions.Add(this);
		SwitchOffAutoDisablingOnInteractableObjects();
	}

	private void SwitchOffInteractions()
	{
//		Collider[] colliders = GetComponentsInChildren<Collider>();
//		
//		foreach (var coll in colliders)
//		{
//			coll.enabled = false;
//		}
		
		foreach (var componentType in interactionComponentTypes)
		{
			Component[] interactionComponentsInChildrens = GetComponentsInChildren(componentType);

			foreach (Component component in interactionComponentsInChildrens)
			{
				MonoBehaviour monoBehaviour = (MonoBehaviour) component;
				monoBehaviour.enabled = false;
			}
		}
	}
	
	private void SwitchOnInteractions()
	{
//		Collider[] colliders = GetComponentsInChildren<Collider>();
//		
//		foreach (var coll in colliders)
//		{
//			coll.enabled = true;
//		}
		
		foreach (var componentType in interactionComponentTypes)
		{
			Component[] interactionComponentsInChildrens = GetComponentsInChildren(componentType);

			foreach (Component component in interactionComponentsInChildrens)
			{
				MonoBehaviour monoBehaviour = (MonoBehaviour) component;
				monoBehaviour.enabled = true;
			}
		}
	}

	private void SwitchOffAutoDisablingOnInteractableObjects()
	{
		VRTK_InteractableObject[] interactableObjects = GetComponentsInChildren<VRTK_InteractableObject>();

		foreach (var interactableObject in interactableObjects)
		{
			interactableObject.disableWhenIdle = false;
		}
	}
}
