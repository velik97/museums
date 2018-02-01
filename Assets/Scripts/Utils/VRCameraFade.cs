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

    public Image fadeImage;                     // Reference to the image that covers the screen.
    public Color fadeColor = Color.black;       // The colour the image fades out to.
    public AnimationCurve fadeCurve;
    public float fadeDuration = 2.0f;           // How long it takes to fade in seconds.
    public bool fadeInOnSceneLoad = false;      // Whether a fade in should happen as soon as the scene is loaded.
    public bool fadeInOnStart = false;          // Whether a fade in should happen just but Updates start.

    private bool isFading;                                        // Whether the screen is currently fading.
    private float fadeStartTime;                                  // The time when fading started.
    private Color fadeOutColor;                                   // This is a transparent version of the fade colour, it will ensure fading looks normal.

    private void Awake()
    {
        fadeOutColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        fadeImage.enabled = true;
    }

    private void Start()
    {
        // If applicable set the immediate colour to be faded out and then fade in.
        if (fadeInOnStart)
        {
            fadeImage.color = fadeColor;
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
        if (fadeInOnSceneLoad)
        {
            fadeImage.color = fadeColor;
            FadeIn();
        }
    }

    public void SetColorIndex(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                fadeColor = Color.white;
                fadeOutColor = new Color(1,1,1,0);
                break;
            case 1:
                fadeColor = Color.black;
                fadeOutColor = new Color(0,0,0,0);
                break;
            case 2:
                fadeColor = new Color(178,0,0,1);
                fadeOutColor = new Color(178, 0, 0, 0);
                break;
            case 3:
                fadeColor = Color.black;
                fadeOutColor = Color.white;
                break;
            case 4:
                fadeColor = Color.black;
                fadeOutColor = fadeColor;
                break;
        }
    }

    public void SetDuration(float duration)
    {
        fadeDuration = duration;
    }

    public void StopFade(Color newFade)
    {
        StopAllCoroutines();
        fadeImage.color = newFade;
        fadeColor = newFade;
    }

    // Since no duration is specified with this overload use the default duration.
    public void FadeOut()
    {
        FadeOut(fadeDuration);
    }


    public void FadeOut(float duration)
    {
        // If not already fading start a coroutine to fade from the fade out colour to the fade colour.

        StopAllCoroutines();

        StartCoroutine(BeginFade(fadeOutColor, fadeColor, duration));
    }

    public void FadeOut(float duration, Color fadeOutColor)
    {

        // If not already fading start a coroutine to fade from the fade out colour to the fade colour.

        StopAllCoroutines();

        StartCoroutine(BeginFade(Color.clear, fadeOutColor, duration));
    }

    public void FadeOut(Color fadeOutColor)
    {
        FadeOut(fadeDuration, fadeOutColor);
    }


    // Since no duration is specified with this overload use the default duration.
    public void FadeIn()
    {
        FadeIn(fadeDuration);
    }


    public void FadeIn(float duration)
    {
        // If not already fading start a coroutine to fade from the fade colour to the fade out colour.
        StopAllCoroutines();

        StartCoroutine(BeginFade(fadeColor, fadeOutColor, duration));
    }

    public IEnumerator BeginFadeOut()
    {

        yield return StartCoroutine(BeginFade(fadeOutColor, fadeColor, fadeDuration));
    }


    public IEnumerator BeginFadeOut(float duration)
    {
        yield return StartCoroutine(BeginFade(fadeOutColor, fadeColor, duration));
    }


    public IEnumerator BeginFadeIn()
    {
        yield return StartCoroutine(BeginFade(fadeColor, fadeOutColor, fadeDuration));
    }


    public IEnumerator BeginFadeIn(float duration)
    {
        yield return StartCoroutine(BeginFade(fadeColor, fadeOutColor, duration));
    }


    private IEnumerator BeginFade(Color startCol, Color endCol, float duration)
    {
        // Fading is now happening.  This ensures it won't be interupted by non-coroutine calls.
        isFading = true;

        // Execute this loop once per frame until the timer exceeds the duration.
        float timer = 0f;

        while (timer <= duration)
        {
            // Set the colour based on the normalised time.
            fadeImage.color = Color.Lerp(startCol, endCol, fadeCurve.Evaluate(timer / duration));

            // Increment the timer by the time between frames and return next frame.
            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = endCol;

        // Fading is finished so allow other fading calls again.
        isFading = false;

        // If anything is subscribed to OnFadeComplete call it.
        if (OnFadeComplete != null)
            OnFadeComplete();
    }
}
