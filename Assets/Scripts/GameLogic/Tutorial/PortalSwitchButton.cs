using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class PortalSwitchButton : MonoSingleton<PortalSwitchButton>
{
	public float localYCoordWhenShow;
	public float localYCoordWhenHide;
	public float moveTime;
	public AnimationCurve moveCurve;
	public Transform baseTransform;
	
	private VRTK_Button_UnityEvents buttonEvents;

	private void Start()
	{
		buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
		if (buttonEvents == null)
		{
			buttonEvents = gameObject.AddComponent<VRTK_Button_UnityEvents>();
		}
				
		TutorialManager.Instance.onTutorialDone.AddListener(Show);
	}

	public void ReactOnButton(object sender, Control3DEventArgs e)
	{		
		DoorAutoClose.Instance.CloseDoor(PortalSwitch.Instance.SwitchPortal);
	}

	public void Show()
	{
		StartCoroutine(MoveButton(localYCoordWhenShow));
		buttonEvents.OnPushed.AddListener(ReactOnButton);
	}

	public void Hide()
	{
		StartCoroutine(MoveButton(localYCoordWhenHide));
		buttonEvents.OnPushed.RemoveAllListeners();
	}

	private IEnumerator MoveButton(float endYCoord)
	{
		Vector3 startPosition = baseTransform.localPosition;
		Vector3 endPosition = startPosition;
		endPosition.y = endYCoord;

		float t = 0;
		float startTime = Time.time;
		
		while ((t = (Time.time - startTime) / moveTime) < 1f)
		{
			baseTransform.localPosition = Vector3.Lerp(startPosition, endPosition, moveCurve.Evaluate(t));
			yield return null;
		}

		baseTransform.localPosition = endPosition;
	}

}
