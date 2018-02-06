using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CollectQuestManager : MonoBehaviour
{
    public List<Basket> baskets;
    public Text pointsText;
    
    public FloatingFeedback feedbackPrefab;
    public Color successColor;
    public Transform feedbackBirthPlace;

    public int bonusForCompleteBasket;
    public int bonusForAllCompeteBaskets;

    private bool questIsDone;

    private int collectedPoints;

    public static CollectQuestManager current;

    public bool[] completedBaskets;
    
    public void StartQuest(float time)
    {
        current = this;
        
        questIsDone = false;

        collectedPoints = 0;
                
        foreach (Basket b in baskets)
        {
            b.onObjectPlacedInBasket.AddListener(CheckAllBaskets);
        }
        
        Timer.Instance.onTimeEnded.AddListener(delegate
        {
            Done();
            Timer.Instance.onTimeEnded.RemoveAllListeners();
        });
        
        pointsText.text = collectedPoints.ToString();
        
        completedBaskets = new bool[baskets.Count];
        for (var i = 0; i < completedBaskets.Length; i++)
        {
            completedBaskets[i] = false;
        }
    }

    private void CheckAllBaskets()
    {
        bool allDone = true;
        for (var i = 0; i < baskets.Count; i++)
        {
            if (!completedBaskets[i])
            {
                if (!baskets[i].BasketIsFull(out completedBaskets[i]))
                {
                    allDone = false;
                }

                if (completedBaskets[i])
                {
                    ShowSuccess(bonusForCompleteBasket, "Complete basket " + baskets[i].name);
                    AddPoints(bonusForCompleteBasket);
                }
            }
        }        

        if (allDone)
        {
            Done();
        }
    }

    public void AddPoints(int points)
    {
        collectedPoints += points;

        if (collectedPoints < 0)
            collectedPoints = 0;
        
        pointsText.text = collectedPoints.ToString();
    }
    
    private void ShowSuccess(int points, string name)
    {
        FloatingFeedback feedback = Instantiate(feedbackPrefab, feedbackBirthPlace) as FloatingFeedback;

        feedback.Set(points, name, successColor, true);
    }
    
    private void Done()
    {                        
        if (questIsDone)
            return;
        
        questIsDone = true;
        Timer.Instance.StopTimer();
        
        foreach (var basket in baskets)
        {
            basket.Deactivate();
        }

        StartCoroutine(WaitBeforeDone());
        
        Debug.Log("Done");
    }

    private IEnumerator WaitBeforeDone()
    {
        yield return new WaitForSeconds(1.5f);       
        
        bool allCompeted = true;
        for (var i = 0; i < completedBaskets.Length; i++)
        {
            if (!completedBaskets[i])
            {
                allCompeted = false;
                break;
            }
        }

        if (allCompeted)
        {
            ShowSuccess(bonusForAllCompeteBaskets, "Complete all baskets");
            AddPoints(bonusForAllCompeteBaskets);            
        }
        
        GameInfo.Instance.points = collectedPoints;
        GameManager.Instance.GameOver();
    }
}
