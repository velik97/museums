﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VRCameraFade))]
public class VRCameraText : MonoSingleton<VRCameraText>
{
    public Color finalColor;
    public float fadeTime;
    public string fadeText;

    public AnimationCurve fadeCurve;

    private VRCameraFade fade;
    
    public Text m_text;

    private bool isFading = false;

    private void Awake()
    {
        fade = GetComponent<VRCameraFade>();
        if (m_text == null)
            m_text = fade.fadeImage.GetComponentInChildren<Text>();
    }
    
    public void SetColorIndex(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                finalColor = Color.white;
                break;
            case 1:
                finalColor = Color.black;
                break;
            case 2:
                finalColor = new Color(178,0,0,1);
                break;
            case 3:
                finalColor = Color.black;
                break;
        }
    }

    public void SetFadeTime(float time)
    {
        fadeTime = time;
    }

    public void ShowText(string text)
    {
        if (isFading == false)
        {
            fadeText = text;
            m_text.text = fadeText;
            StartCoroutine(TextFadeIn());
        }
    }

    public void ShowText()
    {
        if (isFading == false)
        {
            m_text.text = fadeText;
            StartCoroutine(TextFadeIn());
        }
    }
    
    public void HideText()
    {
        if (isFading == false)
        {
            StartCoroutine(TextFadeOut());
        }
    }


    private IEnumerator TextFadeIn()
    {
        isFading = true;

        float startTime = Time.time;
        float currenTime = Time.time - startTime;
        Color currentColor = new Color(finalColor.r, finalColor.g, finalColor.b, fadeCurve.Evaluate(currenTime / fadeTime));

        while ((currenTime / fadeTime) < 1)
        {
            m_text.color = new Color(finalColor.r, finalColor.g, finalColor.b, fadeCurve.Evaluate(currenTime / fadeTime));
            currenTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        }

        m_text.color = finalColor;

        isFading = false;
    }

    private IEnumerator TextFadeOut()
    {
        isFading = true;

        float startTime = Time.time;
        float currenTime = Time.time - startTime;
        Color currentColor = new Color(finalColor.r, finalColor.g, finalColor.b, 1 - fadeCurve.Evaluate(currenTime / fadeTime));

        while ((currenTime / fadeTime) < 1)
        {
            m_text.color = new Color(finalColor.r, finalColor.g, finalColor.b, 1 - fadeCurve.Evaluate(currenTime / fadeTime));
            currenTime = Time.time - startTime;
            yield return new WaitForEndOfFrame();
        }

        m_text.color = new Color(finalColor.r, finalColor.g, finalColor.b, 0);

        isFading = false;
    }
}
