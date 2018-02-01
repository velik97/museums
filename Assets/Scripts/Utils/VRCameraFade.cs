﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// This class is used to fade the entire screen to black (or
// any chosen colour).  It should be used to smooth out the
// transition between scenes or restarting of a scene.
public class VRCameraFade : MonoSingleton<VRCameraFade>
{
    public event Action OnFadeComplete;                             // This is called when the fade in or out has finished.

    [SerializeField] private Image m_FadeImage;                     // Reference to the image that covers the screen.
    [SerializeField] private Color m_FadeColor = Color.black;       // The colour the image fades out to.
    [SerializeField] private AnimationCurve m_FadeCurve;
    [SerializeField] private float m_FadeDuration = 2.0f;           // How long it takes to fade in seconds.
    [SerializeField] private bool m_FadeInOnSceneLoad = false;      // Whether a fade in should happen as soon as the scene is loaded.
    [SerializeField] private bool m_FadeInOnStart = false;          // Whether a fade in should happen just but Updates start.

    private bool m_IsFading;                                        // Whether the screen is currently fading.
    private float m_FadeStartTime;                                  // The time when fading started.
    private Color m_FadeOutColor;                                   // This is a transparent version of the fade colour, it will ensure fading looks normal.


    public Image FadeImage { get { return m_FadeImage; } }
    public bool IsFading { get { return m_IsFading; } }
    public float FadeTime { get { return m_FadeDuration; } }


    private void Awake()
    {
        m_FadeOutColor = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 0f);
        m_FadeImage.enabled = true;
    }

    private void Start()
    {
        // If applicable set the immediate colour to be faded out and then fade in.
        if (m_FadeInOnStart)
        {
            m_FadeImage.color = m_FadeColor;
            FadeIn();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // If applicable set the immediate colour to be faded out and then fade in.
        if (m_FadeInOnSceneLoad)
        {
            m_FadeImage.color = m_FadeColor;
            FadeIn();
        }
    }

    public void SetColorIndex(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                m_FadeColor = Color.white;
                m_FadeOutColor = new Color(1,1,1,0);
                break;
            case 1:
                m_FadeColor = Color.black;
                m_FadeOutColor = new Color(0,0,0,0);
                break;
            case 2:
                m_FadeColor = new Color(178,0,0,1);
                m_FadeOutColor = new Color(178, 0, 0, 0);
                break;
            case 3:
                m_FadeColor = Color.black;
                m_FadeOutColor = Color.white;
                break;
            case 4:
                m_FadeColor = Color.black;
                m_FadeOutColor = m_FadeColor;
                break;
        }
    }

    public void SetDuration(float duration)
    {
        m_FadeDuration = duration;
    }

    public void StopFade(Color newFade)
    {
        StopAllCoroutines();
        m_FadeImage.color = newFade;
        m_FadeColor = newFade;
    }

    // Since no duration is specified with this overload use the default duration.
    public void FadeOut()
    {
        FadeOut(m_FadeDuration);
    }


    public void FadeOut(float duration)
    {
        // If not already fading start a coroutine to fade from the fade out colour to the fade colour.

        StopAllCoroutines();

        StartCoroutine(BeginFade(m_FadeOutColor, m_FadeColor, duration));
    }

    public void FadeOut(float duration, Color fadeOutColor)
    {

        // If not already fading start a coroutine to fade from the fade out colour to the fade colour.

        StopAllCoroutines();

        StartCoroutine(BeginFade(Color.clear, fadeOutColor, duration));
    }

    public void FadeOut(Color fadeOutColor)
    {
        FadeOut(m_FadeDuration, fadeOutColor);
    }


    // Since no duration is specified with this overload use the default duration.
    public void FadeIn()
    {
        FadeIn(m_FadeDuration);
    }


    public void FadeIn(float duration)
    {
        Debug.Log("fade in");
        // If not already fading start a coroutine to fade from the fade colour to the fade out colour.
        StopAllCoroutines();

        StartCoroutine(BeginFade(m_FadeColor, m_FadeOutColor, duration));
    }

    public IEnumerator BeginFadeOut()
    {

        yield return StartCoroutine(BeginFade(m_FadeOutColor, m_FadeColor, m_FadeDuration));
    }


    public IEnumerator BeginFadeOut(float duration)
    {
        yield return StartCoroutine(BeginFade(m_FadeOutColor, m_FadeColor, duration));
    }


    public IEnumerator BeginFadeIn()
    {
        yield return StartCoroutine(BeginFade(m_FadeColor, m_FadeOutColor, m_FadeDuration));
    }


    public IEnumerator BeginFadeIn(float duration)
    {
        yield return StartCoroutine(BeginFade(m_FadeColor, m_FadeOutColor, duration));
    }


    private IEnumerator BeginFade(Color startCol, Color endCol, float duration)
    {
        // Fading is now happening.  This ensures it won't be interupted by non-coroutine calls.
        m_IsFading = true;

        // Execute this loop once per frame until the timer exceeds the duration.
        float timer = 0f;

        while (timer <= duration)
        {
            // Set the colour based on the normalised time.
            m_FadeImage.color = Color.Lerp(startCol, endCol, m_FadeCurve.Evaluate(timer / duration));

            // Increment the timer by the time between frames and return next frame.
            timer += Time.deltaTime;
            yield return null;
        }

        m_FadeImage.color = endCol;

        // Fading is finished so allow other fading calls again.
        m_IsFading = false;

        // If anything is subscribed to OnFadeComplete call it.
        if (OnFadeComplete != null)
            OnFadeComplete();
    }
}
