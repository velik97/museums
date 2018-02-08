using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class SlideShow : MonoSingleton<SlideShow>
{
    public GameObject[] slides;    
    public float successfullSlideShowTime;

    public VRTK_InteractGrab[] grabs;

    private int currentSlideNumber;    

    private void Awake()
    {
        currentSlideNumber = 0;
        for (var i = 1; i < slides.Length; i++)
        {
            slides[i].SetActive(false);
        }
        slides[0].SetActive(true);
        
        foreach (var g in grabs)
        {
            var e = g.GetComponent<VRTK_InteractGrab_UnityEvents>();
            if (e == null)
                e = g.gameObject.AddComponent<VRTK_InteractGrab_UnityEvents>();
            
            e.OnControllerGrabInteractableObject.AddListener(delegate(object sender, ObjectInteractEventArgs args)
            {
                if (currentSlideNumber == 0 && args.target.GetComponent<TutorialPickUpObject>() != null)
                    SwitchSlide();
            });
        }
        
        TutorialManager.Instance.onTutorialDone.AddListener(delegate
        {
            if (currentSlideNumber == 1)
                SwitchSlide();
        });
    }

    public void SwitchSlide()
    {
        SwitchSlide(currentSlideNumber + 1);
    }
    
    public void SwitchSlide(int slideNumber)
    {
        StartCoroutine(ShowSuccessfSlide(slideNumber));
    }

    private IEnumerator ShowSuccessfSlide(int slideNumber)
    {
        slides[currentSlideNumber].SetActive(false);
        currentSlideNumber = slideNumber;
        yield return new WaitForSeconds(successfullSlideShowTime);
        
        if (currentSlideNumber < slides.Length)
            slides[currentSlideNumber].SetActive(true);
    }
}
