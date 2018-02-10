using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Basket : MonoBehaviour
{
    public Text overallCountText;
    
    public FloatingFeedback feedbackPrefab;
    public Color successColor;
    public Color failureColor;

    public Transform feedbackBirthPlace;

    public GameObject cover;
    
    public UnityEvent onObjectPlacedInBasket;
    
    public Museum museum;    

    private int inBasketObjectsCount;
    [HideInInspector] public int overAllObjectsCount;
    [HideInInspector] public int initialObjectsCount;

    private bool noMorePickUpsByMyIndex;

    private static Dictionary<Museum, Basket> basketByIndex;

    private PickUpObject lastPickUpObject;

    private void Awake()
    {
        WaveGameSetter.SubscribeOnWave(SetBasketsDictionary, 2);
        WaveGameSetter.SubscribeOnWave(SetBasket, 2);
        
        // TODO prototype line ===================================
        WaveGameSetter.SubscribeOnWave(delegate
        {
            if (GetComponent<PrototypeMuseumMaterials>() != null)
                GetComponent<PrototypeMuseumMaterials>().SetMaterial(museum);
        }, 2);
        // =======================================================
    }

    public static void ResetList()
    {
        print("reset baskets");
        basketByIndex.Clear();
    }

    private void SetBasketsDictionary()
    {
        if (basketByIndex == null)
            basketByIndex = new Dictionary<Museum, Basket>();

        if (!basketByIndex.ContainsKey(museum))
        {
            basketByIndex.Add(museum, this);
        }
        else
        {
            Debug.LogError("More than 1 basket with index: " + museum + "!");
        }
    }

    private void SetBasket()
    {
        inBasketObjectsCount = 0;
        overAllObjectsCount = PickUpObject.PickUpsListByIndex[museum].Count;
        initialObjectsCount = overAllObjectsCount;
        
        SetCountText();

        noMorePickUpsByMyIndex = false;
        
        cover.SetActive(false);
    }

    public void Deactivate()
    {
        cover.SetActive(true);
        UnityEngine.BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
            boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var pu = other.GetComponent<PickUpObject>();
        if (pu != null)
            CountNewPicjUpObject(pu);
    }

    private void CountNewPicjUpObject(PickUpObject pu)
    {
        if (lastPickUpObject == pu)
            return;

        lastPickUpObject = pu;
        
        if (pu.artefactInfo.museum == museum)
        {
            inBasketObjectsCount++;

            pu.PlaceInBusket(true);            
            ShowSuccess(pu.artefactInfo.points, pu.artefactInfo.name);
            SetCountText();
            
            CollectQuestManager.current.AddPoints(pu.artefactInfo.points);
            GameInfo.Instance.OnArtefactCollecetd(pu.id);
        }
        else
        {                   
            pu.PlaceInBusket(true);
            ShowFailure(pu.artefactInfo.points, pu.artefactInfo.name);

            if (basketByIndex.ContainsKey(pu.artefactInfo.museum))
            {
                basketByIndex[pu.artefactInfo.museum].overAllObjectsCount--;
                basketByIndex[pu.artefactInfo.museum].SetCountText();
            }
            
            CollectQuestManager.current.AddPoints(-pu.artefactInfo.points);
        }               
          
        onObjectPlacedInBasket.Invoke();        
    }

    public bool BasketIsFull()
    {
        if (!noMorePickUpsByMyIndex && overAllObjectsCount == inBasketObjectsCount)
        {                
            noMorePickUpsByMyIndex = true;
        }

        return noMorePickUpsByMyIndex;
    }
    
    public bool BasketIsFull(out bool successfully)
    {
        if (!noMorePickUpsByMyIndex && overAllObjectsCount == inBasketObjectsCount)
        {                
            noMorePickUpsByMyIndex = true;
        }

        successfully = inBasketObjectsCount == initialObjectsCount;

        return noMorePickUpsByMyIndex;
    }

    public void SetCountText()
    {
        overallCountText.text = inBasketObjectsCount + "/" + overAllObjectsCount;
    }
    
    private void ShowSuccess(int points, string name)
    {
        FloatingFeedback feedback = Instantiate(feedbackPrefab, feedbackBirthPlace) as FloatingFeedback;

        feedback.Set(points, name, successColor, true);
    }

    private void ShowFailure(int points, string name)
    {
        FloatingFeedback feedback = Instantiate(feedbackPrefab, feedbackBirthPlace) as FloatingFeedback;
        
        feedback.Set(points, name, failureColor, false);
    }
}

public class IntEvent : UnityEvent<int> {}