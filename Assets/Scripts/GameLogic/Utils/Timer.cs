using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text text;
    public UnityEvent onTimeEnded;

    private int minutes;
    private int seconds;
    private int prevSeconds;

    private float timeToEnd;

    private bool active;

    private void Awake()
    {
        text.text = "00:00";
        prevSeconds = 0;
        seconds = 0;
        minutes = 0;

        active = false;
    }

    private void Update()
    {
        if (!active)
            return;
        
        timeToEnd -= Time.deltaTime;

        if (timeToEnd < 0f)
        {
            active = false;
            timeToEnd = 0f;
        }

        seconds = Mathf.RoundToInt(timeToEnd);
        minutes = seconds / 60;
        seconds %= 60;

        if (seconds != prevSeconds)
            text.text =  minutes.ToString() + ":" + ((seconds > 9) ? "" : "0") + seconds.ToString();

        prevSeconds = seconds;
    }

    public void StartTimer(float time)
    {
        timeToEnd = Mathf.Clamp(time, 0, 3600);

        active = true;
    }

    public void StopTimer()
    {
        active = false;
    }
}
