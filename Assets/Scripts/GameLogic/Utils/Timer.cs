using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoSingleton<Timer>
{
    public List<TimerDisplay> displays;
    public UnityEvent onTimeEnded;

    private int minutes;
    private int seconds;
    private int prevSeconds;

    private float timeToEnd;

    private bool active;

    private string currentText;

    private void Awake()
    {
        SetAllDisplays("");
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
            onTimeEnded.Invoke();
        }

        seconds = Mathf.RoundToInt(timeToEnd);
        minutes = seconds / 60;
        seconds %= 60;

        if (seconds != prevSeconds)
            SetAllDisplays(minutes.ToString() + ":" + ((seconds > 9) ? "" : "0") + seconds.ToString());

        prevSeconds = seconds;
    }

    public void AddDisplay(TimerDisplay display)
    {
        if (displays == null)
            displays = new List<TimerDisplay>();
        
        displays.Add(display);
        display.SetText(currentText);
    }

    private void SetAllDisplays(string text)
    {
        if (displays == null)
            return;

        currentText = text;
        
        foreach (var d in displays)
        {
            d.SetText(text);
        }
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
