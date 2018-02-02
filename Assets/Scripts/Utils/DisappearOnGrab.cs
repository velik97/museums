using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class DisappearOnGrab : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;
    private VRTK_InteractGrab_UnityEvents grabEvents;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        
        grabEvents = GetComponent<VRTK_InteractGrab_UnityEvents>();
        if (grabEvents == null)
        {
            grabEvents = gameObject.AddComponent<VRTK_InteractGrab_UnityEvents>();
        }
        
        grabEvents.OnControllerGrabInteractableObject.AddListener(Disappear);
        grabEvents.OnControllerUngrabInteractableObject.AddListener(Appear);
    }

    private void Disappear(object sender, ObjectInteractEventArgs args)
    {
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
    
    private void Appear(object sender, ObjectInteractEventArgs args)
    {
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }
    }
}
