using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	public string startText;
	
	public Transform doorTransform;
	public List<Transform> trackedObjects;
	public float minDistanceToDelete;

	public List<GameObject> gameObjectsToDeleteAfterTutorial;
	public GameObject door;

	private void Start()
	{
		VRCameraText.Instance.ShowText(startText);
		Invoke("StartGame", 5f);
	}

	public void StartGame()
	{
		DimensionExtended.StartInteractionsWithCurrent();
		VRCameraText.Instance.HideText();
		VRCameraFade.Instance.FadeOut();
	}
	
	private void Update()
	{
		CheckPosition();
	}

	private void CheckPosition()
	{
		if (DimensionExtended.initial == null)
			return;
		
		if (DimensionExtended.Current == DimensionExtended.initial)
			return;
		
		foreach (var trackedObject in trackedObjects)
		{
			Vector3 dif3 = doorTransform.position - trackedObject.position;
			Vector2 dif2 = new Vector2(dif3.x, dif3.z);
			
			if (Vector2.SqrMagnitude(dif2) > minDistanceToDelete * minDistanceToDelete)
			{
				DoorAutoClose.Instance.onDoorClosed.AddListener(delegate
				{
					StartCoroutine(DeleteObjectsAndStartQuest());
				});
				DoorAutoClose.Instance.CloseDoor();
				break;
			}
		}
	}

	private IEnumerator DeleteObjectsAndStartQuest()
	{
		foreach (var obj in gameObjectsToDeleteAfterTutorial)
		{
			DimensionExtended dimension = obj.GetComponent<DimensionExtended>();
			
			if (dimension == null || dimension != DimensionExtended.Current)
			{
				obj.SetActive(false);
			}		
		}

		GameInfo.Instance.locationId = DimensionExtended.Current.index;
		
		door.GetComponent<Animator>().SetTrigger("Disolve");
		
		yield return new WaitForSeconds(1f);
		door.SetActive(false);
		
		this.gameObject.SetActive(false);
		
		DimensionExtended.Current.GetComponent<CollectQuestManager>().StartQuest();
	}
}
