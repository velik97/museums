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

    public List<Material> materials;
    public List<MeshRenderer> colorMeshRenderers;
    
    public UnityEvent onPickUpsWithMyIndexOver;
    public UnityEvent onObjectPlacedInBasket;
    
    public int index;

    private static IntEvent onPickUpDestroyedWithFailure;

    private int inBasketCount;
    private int overAllCount;

    [HideInInspector] public int inBasketPoints;
    [HideInInspector] public int overallPoints;

    private bool noMorePickUpsByMyIndex;

    private void Awake()
    {
        if (onPickUpDestroyedWithFailure == null)
            onPickUpDestroyedWithFailure = new IntEvent();
        
        onPickUpDestroyedWithFailure.AddListener(delegate(int destroyedIndex)
        {
            if (index == destroyedIndex)
                overAllCount--;
            
            SetCountText();
        });        
    }

    private void Start()
    {
        inBasketPoints = 0;
        overallPoints = 0;

        inBasketCount = 0;
        overAllCount = PickUpObject.PickUpsListByIndex[index].Count;
        
        foreach (var pickUp in PickUpObject.PickUpsListByIndex[index])
        {
            overallPoints += pickUp.points;
        }
        
        SetCountText();

        noMorePickUpsByMyIndex = false;
        
        cover.SetActive(false);
        
        SetMaterial();        
    }

    private void SetMaterial()
    {
        foreach (var colorMeshRenderer in colorMeshRenderers)
        {
            colorMeshRenderer.material = materials[(index - 1) % materials.Count];
        }
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
        onObjectPlacedInBasket.Invoke();
        
        if (pu.index == index)
        {
            inBasketPoints += pu.points;
            inBasketCount++;

            pu.PlaceInBusket(true);
            if (!noMorePickUpsByMyIndex && PickUpObject.PickUpsListByIndex[index].Count == 0)
            {                
                noMorePickUpsByMyIndex = true;
                onPickUpsWithMyIndexOver.Invoke();
            }
            ShowSuccess(pu.points, pu.pickUpName);
            SetCountText();
        }
        else
        {           
            inBasketPoints -= pu.points;
            inBasketPoints = inBasketPoints > 0 ? inBasketPoints : 0;
            pu.PlaceInBusket(true);
            ShowFailure(pu.points, pu.pickUpName);
            onPickUpDestroyedWithFailure.Invoke(pu.index);
        }               
                
    }

    private void SetCountText()
    {
        overallCountText.text = inBasketCount + "/" + overAllCount;
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
