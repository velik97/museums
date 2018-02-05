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
    
    public UnityEvent onObjectPlacedInBasket;
    
    public int index;    

    private int inBasketObjectsCount;
    [HideInInspector] public int overAllObjectsCount;

    private bool noMorePickUpsByMyIndex;

    private static Dictionary<int, Basket> basketByIndex;

    private PickUpObject lastPickUpObject;

    private void Awake()
    {
        if (basketByIndex == null)
            basketByIndex = new Dictionary<int, Basket>();

        if (!basketByIndex.ContainsKey(index))
        {
            basketByIndex.Add(index, this);
        }
        else
        {
            Debug.LogError("More than 1 basket with index: " + index + "!");
        }
    }

    private void Start()
    {        
        inBasketObjectsCount = 0;
        overAllObjectsCount = PickUpObject.PickUpsListByIndex[index].Count;
        
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
        if (lastPickUpObject == pu)
            return;

        lastPickUpObject = pu;
        
        if (pu.index == index)
        {
            inBasketObjectsCount++;

            pu.PlaceInBusket(true);            
            ShowSuccess(pu.points, pu.pickUpName);
            SetCountText();
            
            CollectQuestManager.current.AddPoints(pu.points);
        }
        else
        {                   
            pu.PlaceInBusket(true);
            ShowFailure(pu.points, pu.pickUpName);

            if (basketByIndex.ContainsKey(pu.index))
            {
                basketByIndex[pu.index].overAllObjectsCount--;
                basketByIndex[pu.index].SetCountText();
            }
            
            CollectQuestManager.current.AddPoints(-pu.points);
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