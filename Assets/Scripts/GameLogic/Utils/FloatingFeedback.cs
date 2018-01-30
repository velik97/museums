using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingFeedback : MonoBehaviour
{
	public Text pickUpNameText;
	public Text pointsText;

	public void Set(int points, string pickUpName, Color color, bool success)
	{
		pointsText.text = (success ? "+" : "-") + points.ToString();
		pickUpNameText.text = pickUpName;

		pointsText.color = color;
	}
}
