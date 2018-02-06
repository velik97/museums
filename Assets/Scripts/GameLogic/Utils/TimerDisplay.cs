using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    public Text text;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<Text>();
        
        Timer.Instance.AddDisplay(this);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
