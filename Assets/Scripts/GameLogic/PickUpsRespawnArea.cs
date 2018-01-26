using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpsRespawnArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PickUpObject pu = other.GetComponent<PickUpObject>();
        
        if (pu != null)
            pu.PlaceInBusket(false);
    }

    private void OnTriggerStay(Collider other)
    {
        PickUpObject pu = other.GetComponent<PickUpObject>();
        
        if (pu != null)
            pu.PlaceInBusket(false);
    }
}
