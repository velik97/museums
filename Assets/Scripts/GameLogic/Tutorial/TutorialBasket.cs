using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialBasket : MonoBehaviour
{
    public int index;

    public GameObject successPrefab;
    public GameObject failurePrefab;

    public Transform feedbackBirthPlace;

    public UnityEvent onBasketFull;

    [HideInInspector] public int inBasketCount;
    [HideInInspector] public int overallCount;

    private bool isFull;

    private void Start()
    {
        inBasketCount = 0;
        overallCount = PickUpObject.CountByIndex(index);

        isFull = false;

        if (inBasketCount == overallCount)
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
            inBasketCount++;
            ShowSuccess();
        }
        else
        {
            ShowFailure();
        }

        pu.PlaceInBusket(pu.index == index);

        if (!isFull && inBasketCount == overallCount)
        {
            isFull = true;
            onBasketFull.Invoke();
        }
    }

    private void ShowSuccess()
    {
        Instantiate(successPrefab, feedbackBirthPlace);
    }

    private void ShowFailure()
    {
        Instantiate(failurePrefab, feedbackBirthPlace);
    }
}