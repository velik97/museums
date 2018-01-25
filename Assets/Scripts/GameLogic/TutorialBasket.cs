using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBasket : MonoBehaviour
{
	public int index;

	private int points;
	private int overallPoints;
    
	private void Start()
	{
		points = 0;
		overallPoints = PickUpObject.PointsByIndex(index);
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
	}
}
