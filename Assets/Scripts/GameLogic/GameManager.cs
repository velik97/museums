using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
	public string startText = "The game will start soon...";
	
	public Transform doorTransform;
	public List<Transform> trackedObjects;
	public float minDistanceToDelete;
	public float timeForQuest;

	public List<GameObject> gameObjectsToDeleteAfterTutorial;

	public string finalSceneName = "Final";

	private bool tutorialIsDone;

	private void Start()
	{
		VRCameraText.Instance.ShowText(startText);
		StartGame();
	}

	public void StartGame()
	{
		StartCoroutine(FadeInAndStartGame());
		tutorialIsDone = false;
	}

	public void GameOver()
	{
		StartCoroutine(FadeOutAndLoadFinalScene());
	}
	
	private void Update()
	{
		if (tutorialIsDone)
			return;
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
					TutorialDone();
					DoorAutoClose.Instance.onDoorClosed.RemoveAllListeners();
				});
				DoorAutoClose.Instance.CloseDoor();
				break;
			}
		}
	}

	private void TutorialDone()
	{
		StartCoroutine(DeleteObjectsAndStartQuest());
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
		
		doorTransform.GetComponent<Animator>().SetTrigger("Disolve");
		
		yield return new WaitForSeconds(1f);
		doorTransform.gameObject.SetActive(false);
				
		DimensionExtended.Current.GetComponent<CollectQuestManager>().StartQuest(timeForQuest);
	}

	private IEnumerator FadeInAndStartGame()
	{
		yield return new WaitForSeconds(3f);
		DimensionExtended.StartInteractionsWithCurrent();
		VRCameraText.Instance.HideText();
		yield return new WaitForSeconds(2f);
		VRCameraFade.Instance.FadeIn();
	}

	private IEnumerator FadeOutAndLoadFinalScene()
	{
		VRCameraFade.Instance.FadeOut();
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(finalSceneName);
	}
}
