using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basket : MonoBehaviour
{
    public Text overallPointsText;
    public Text flyingPointsTextPrefab;
    
    public int index;

    private int points;
    private int overallPoints;
    
    private void Start()
    {
        points = 0;
        overallPoints = PickUpObject.PointsByIndex(index);
        
        overallPointsText.text = points + "/" + overallPoints;
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
            points += pu.points;
        }
        else
        {
            points -= pu.points;
            points = points > 0 ? points : 0;
        }
        
        overallPointsText.text = points + "/" + overallPoints;

        Destroy(pu.gameObject);
    }
}
