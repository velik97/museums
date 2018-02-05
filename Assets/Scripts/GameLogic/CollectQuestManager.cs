using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private float timeForQuest;

    public static CollectQuestManager current;
    
    public void StartQuest(float time)
    {
        current = this;
        
        doneBasketsCount = 0;
        questIsDone = false;

        collectedPoints = 0;
        overallPoints = 0;

        timeForQuest = time;        
                
        foreach (Basket b in baskets)
        {
            b.onObjectPlacedInBasket.AddListener(CheckAllBaskets);
            foreach (var pu in PickUpObject.PickUpsListByIndex[b.index])
            {
                overallPoints += pu.points;
            }
        }
        
        timer.onTimeEnded.AddListener(delegate
        {
            Done();
            timer.onTimeEnded.RemoveAllListeners();
        });
        
        timer.StartTimer(timeForQuest);

        pointsText.text = collectedPoints + "/" + overallPoints;
    }

    private void CheckAllBaskets()
    {
        if (baskets.All(o => o.BasketIsFull() == true))
        {
            Done();
        }
    }

    public void AddPoints(int points)
    {
        collectedPoints += points;

        if (collectedPoints < 0)
            collectedPoints = 0;
        
        pointsText.text = collectedPoints + "/" + overallPoints;
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
