using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Object = UnityEngine.Object;

public class DimensionExtended : Dimension
{
	public Location location;
	
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

	public static void StartInteractionsWithCurrent()
	{
		Current.SwitchOnInteractions();
	}

	private void Start ()
	{
		if (initialWorld)
		{
			current = this;
			initial = this;			
		}
		
		SwitchOffAutoDisablingOnInteractableObjects();
		SwitchOffInteractions();

		if (allDimensions == null)
			allDimensions = new List<DimensionExtended>();
		
		allDimensions.Add(this);		
	}

	private void SwitchOffInteractions()
	{		
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
