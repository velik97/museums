using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeleteTableDialog : MonoBehaviour
{
    public Button deleteButton;
    public Button cancelButton;

    public Text descriptionText;

    private void Awake()
    {
        cancelButton.onClick.AddListener(Close);
    }

    public void RequestDelete(int locationId, UnityAction callback)
    {
        if (locationId >= 0 && locationId <= 2)
        {
            descriptionText.text = "Вы точно хотите удлаить таблицу \"" + GameInfo.LocationByIndex(locationId) + "\"?";
        }
        else
        {
            descriptionText.text = "Вы точно хотите удлаить всю таблицу рекордов?";
        }
        gameObject.SetActive(true);
        
        deleteButton.onClick.AddListener(callback);
        deleteButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        deleteButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
    
}
