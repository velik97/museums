using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoSingleton<TutorialManager>
{
	public List<TutorialBasket> tutorialBaskets;

	public UnityEvent onTutorialDone;

	private int allBasketsCount;
	private int fullBasketsCount;
	
	private bool tutorialDone;

	private void Awake()
	{
		allBasketsCount = tutorialBaskets.Count;
		fullBasketsCount = 0;

		tutorialDone = false;
		
		for (var i = 0; i < tutorialBaskets.Count; i++)
		{
			tutorialBaskets[i].onPickUpsWithMyIndexOver.AddListener(BasketBecameFull);
		}
		
		onTutorialDone.AddListener(delegate
		{
			print("Tutorial is done");
		});
	}

	private void BasketBecameFull()
	{
		fullBasketsCount++;
		if (!tutorialDone && fullBasketsCount == allBasketsCount)
		{
			tutorialDone = true;
			onTutorialDone.Invoke();
		}
	}
}
