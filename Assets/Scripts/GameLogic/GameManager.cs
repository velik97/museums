using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{		
	public string startText = "The game will start soon...";

	public InputField nameField;
	
	public Transform doorTransform;
	public List<Transform> trackedObjects;
	public float minDistanceToDelete;
	public float timeForQuest;

	public List<GameObject> gameObjectsToDeleteAfterTutorial;

	public UnityEvent onGameOver;

	public string[] finalSceneNames;

	private bool tutorialIsDone;
	private StartTimeOption startTimeOption;

	private void Awake()
	{
		VRCameraText.Instance.ShowText(startText);
		GameInfo.Instance.FreeCollectedArtefactsIdsList();
	}

	public void StartGame()
	{
		GameInfo.Instance.playerName = StartGameUI.Instance.finalPlayerName;
		startTimeOption = (StartTimeOption) StartGameUI.Instance.StartTimeOption;
		print("start time: " + startTimeOption);
		timeForQuest = StartGameUI.Instance.GameMinutesTime * 60;
		StartCoroutine(FadeInAndStartGame());
		tutorialIsDone = false;		
	}

	public void GameOver()
	{
		onGameOver.Invoke();
		PickUpObject.ResetList();
		Basket.ResetList();
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
				DoorAutoClose.Instance.CloseDoor(TutorialDone);
				break;
			}
		}
	}

	private void TutorialDone()
	{
		if (tutorialIsDone)
			return;
		
		tutorialIsDone = true;
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

		GameInfo.Instance.location = DimensionExtended.Current.location;
		
		doorTransform.GetComponent<Animator>().SetTrigger("Disolve");
		
		yield return new WaitForSeconds(1f);
		doorTransform.gameObject.SetActive(false);
		
		Timer.Instance.onTimeEnded.RemoveAllListeners();
		DimensionExtended.Current.GetComponent<CollectQuestManager>().StartQuest(timeForQuest);		
		if (startTimeOption == StartTimeOption.AfetrTutorial)
			Timer.Instance.StartTimer(timeForQuest);
	}

	private IEnumerator FadeInAndStartGame()
	{
		yield return new WaitForSeconds(3f);
		DimensionExtended.StartInteractionsWithCurrent();
		VRCameraText.Instance.HideText();
		yield return new WaitForSeconds(2f);
		VRCameraFade.Instance.FadeIn();

		if (startTimeOption == StartTimeOption.FromStart)
		{
			Timer.Instance.onTimeEnded.AddListener(delegate
			{
				GameOver();
				Timer.Instance.onTimeEnded.RemoveAllListeners();
			});
			Timer.Instance.StartTimer(timeForQuest);
		}
	}

	private IEnumerator FadeOutAndLoadFinalScene()
	{
		VRCameraFade.Instance.FadeOut();
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(finalSceneNames[(int)GameInfo.Instance.location]);
	}
}

public enum StartTimeOption
{
	FromStart = 0,
	AfetrTutorial = 1
}