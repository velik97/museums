using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Basket : MonoBehaviour
{
    public Text overallPointsText;
    
    public GameObject feedbackPrefab;
    public Color successColor;
    public Color failureColor;

    public Transform feedbackBirthPlace;

    public UnityEvent onBasketFull;
    public UnityEvent onObjectPlacedInBasket;
    
    public int index;

    [HideInInspector] public int inBasketPoints;
    [HideInInspector] public int overallPoints;
    
    private int inBasketCount;
    private int overallCount;

    private bool isFull;
    
    private void Start()
    {
        inBasketPoints = 0;
        overallPoints = PickUpObject.PointsByIndex(index);

        inBasketCount = 0;
        overallCount = PickUpObject.CountByIndex(index);
        
        overallPointsText.text = inBasketPoints + "/" + overallPoints;

        isFull = false;
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
            inBasketPoints += pu.points;
            inBasketCount++;

            if (!isFull && inBasketCount == overallCount)
            {
                isFull = true;
                onBasketFull.Invoke();
            }
            ShowSuccess(pu.points);
        }
        else
        {
            inBasketPoints -= pu.points;
            inBasketPoints = inBasketPoints > 0 ? inBasketPoints : 0;
            ShowFailure(pu.points);
        }
        
        overallPointsText.text = inBasketPoints + "/" + overallPoints;
        
        pu.PlaceInBusket(true);
        
        onObjectPlacedInBasket.Invoke();
    }
    
    private void ShowSuccess(int points)
    {
        GameObject feedback = Instantiate(feedbackPrefab, feedbackBirthPlace) as GameObject;

        Text text = feedback.GetComponentInChildren<Text>();

        text.color = successColor;
        text.text = "+" + points;
    }

    private void ShowFailure(int points)
    {
        GameObject feedback = Instantiate(feedbackPrefab, feedbackBirthPlace) as GameObject;
        
        Text text = feedback.GetComponentInChildren<Text>();

        text.color = failureColor;
        text.text = "-" + points;
    }
}
