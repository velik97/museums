using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalPedestal : MonoBehaviour
{
    public FinalPickUp pickUp;
    
    public Text pickUpNameText;
    public Text pickUpDescriptionText;

    private void Awake()
    {
        WaveGameSetter.SubscribeOnWave(SetNameAndDescriprion, 1);
    }
    
    private void SetNameAndDescriprion()
    {
        pickUpNameText.text = pickUp.artefactInfo.name;
        pickUpDescriptionText.text = pickUp.artefactInfo.description;
    }
}
