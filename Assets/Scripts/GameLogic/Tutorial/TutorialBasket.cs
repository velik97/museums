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

    public UnityEvent onPickUpsWithMyIndexOver;

    private void OnTriggerEnter(Collider other)
    {
        var pu = other.GetComponent<TutorialPickUpObject>();
        if (pu != null)
            CountNewPicjUpObject(pu);
    }

    private void CountNewPicjUpObject(TutorialPickUpObject pu)
    {
        if (pu.index == index)
        {
            ShowSuccess();
            onPickUpsWithMyIndexOver.Invoke();
        }
        else
        {
            ShowFailure();
        }

        pu.PlaceInBusket(pu.index == index);
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