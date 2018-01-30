using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
	public Text placeText;
	public Text nameText;
	public Text statusText;
	public Text pointsText;

	public Image bgImage;

	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void SetEmpty()
	{
		placeText.text  = "";
		nameText.text   = "";
		statusText.text = "...";
		pointsText.text = "";
	}

	public void SetInfo (Score score, int place)
	{
		placeText.text  = place.ToString();
		nameText.text   = score.name;
		statusText.text = score.status;
		pointsText.text = score.points.ToString();
	}

	public void SetInfo (Score score, int place, Color bgColor)
	{
		SetInfo (score, place);
		bgImage.color = bgColor;
	}

	public void SetPoisition (float yMin, float yMax)
	{
		rectTransform.anchorMin = new Vector2(0f, yMin);
		rectTransform.anchorMax = new Vector2(1f, yMax);
		
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
	}
}
