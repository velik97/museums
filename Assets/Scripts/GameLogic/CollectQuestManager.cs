using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectQuestManager : MonoBehaviour
{
    public List<Basket> baskets;
    public Timer timer;
    public Text pointsText;

    private int doneBasketsCount;
    private bool questIsDone;

    private int collectedPoints;
    private int overallPoints;

    private int[] pointsInBaskets;

    private float timeForQuest;
    
    public void StartQuest(float time)
    {
        doneBasketsCount = 0;
        questIsDone = false;

        collectedPoints = 0;
        overallPoints = 0;

        timeForQuest = time;
        
        pointsInBaskets = new int[baskets.Count];

        print("start quest");
                
        for (var i = 0; i < baskets.Count; i++)
        {
            int copy = i;
            baskets[i].onPickUpsWithMyIndexOver.AddListener(BasketDone);

            overallPoints += baskets[i].overallPoints;
            pointsInBaskets[i] = 0;
            
            baskets[i].onObjectPlacedInBasket.AddListener(delegate
            {
                SetPoints(copy);
            });
        }
        
        timer.onTimeEnded.AddListener(delegate
        {
            Done();
            timer.onTimeEnded.RemoveAllListeners();
        });
        
        timer.StartTimer(timeForQuest);

        pointsText.text = collectedPoints + "/" + overallPoints;
    }

    private void SetPoints(int basketNumber)
    {
        pointsInBaskets[basketNumber] = baskets[basketNumber].inBasketPoints;
        collectedPoints = 0;
        
        foreach (var points in pointsInBaskets)
        {
            collectedPoints += points;
        }
        
        pointsText.text = collectedPoints + "/" + overallPoints;
    }
    
    private void BasketDone()
    {
        doneBasketsCount++;

        print(2 + " done count: " + doneBasketsCount + ", all count: " + baskets.Count); 
        
        if (doneBasketsCount == baskets.Count)
            Done();
    }
    
    private void Done()
    {
        if (questIsDone)
            return;
        
        questIsDone = true;
        timer.StopTimer();
        
        foreach (var basket in baskets)
        {
            basket.Deactivate();
        }

        GameInfo.Instance.points = collectedPoints;
        GameManager.Instance.GameOver();
        
        Debug.Log("Done");
    }
}
