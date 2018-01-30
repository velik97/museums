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

    private bool noMorePickUpsByMyIndex;

    private void Start()
    {
        noMorePickUpsByMyIndex = false;

        if (PickUpObject.PickUpsListByIndex[index].Count == 0)
        {
            noMorePickUpsByMyIndex = true;
            onPickUpsWithMyIndexOver.Invoke();
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
            ShowSuccess();
        }
        else
        {
            ShowFailure();
        }

        pu.PlaceInBusket(pu.index == index);

        if (!noMorePickUpsByMyIndex && PickUpObject.PickUpsListByIndex[index].Count == 0)
        {
            noMorePickUpsByMyIndex = true;
            onPickUpsWithMyIndexOver.Invoke();
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