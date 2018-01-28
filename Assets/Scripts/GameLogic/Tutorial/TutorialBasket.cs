using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialBasket : MonoBehaviour
{
	public int index;

	public UnityEvent onBasketFull;

	private int points;
	private int overallPoints;

	private bool isFull;
    
	private void Start()
	{
		points = 0;
		overallPoints = PickUpObject.PointsByIndex(index);

		isFull = false;

		if (points == overallPoints)
		{
			isFull = true;
			onBasketFull.Invoke();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var pu = other.GetComponent<PickUpObject>();
		if (pu != null)
			CountNewPicjUpObject(pu);
	}

	private void CountNewPicjUpObject(PickUpObject pu)
	{
		if (pu.index == index)
		{
			points += pu.points;
		}
		
		pu.PlaceInBusket(pu.index == index);

		if (!isFull && points == overallPoints)
		{
			isFull = true;
			onBasketFull.Invoke();
		}
	}
}
