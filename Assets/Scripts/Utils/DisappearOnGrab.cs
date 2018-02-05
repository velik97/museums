using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class DisappearOnGrab : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;
    private VRTK_InteractGrab grab;

    private bool grabbing;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        grab = GetComponent<VRTK_InteractGrab>();
        grabbing = false;
    }

    private void Update()
    {
        if (grabbing)
        {
            if (grab.GetGrabbedObject() == null)
            {
                grabbing = false;
                Appear();
            }
        }
        else
        {
            if (grab.GetGrabbedObject() != null)
            {
                grabbing = true;
                Disappear();
            }
        }
    }

    private void Disappear()
    {       
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
    
    private void Appear()
    {
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }
    }
}
