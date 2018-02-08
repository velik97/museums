using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_InteractableObject))]
[RequireComponent(typeof(HingeJoint))]
public class DoorAutoClose : MonoSingleton<DoorAutoClose>
{
    public float minAngleDifferenceToStopClosing = 1f;
    public float minAngleToAutoClose = 15f;
    
    private HingeJoint joint;
    private VRTK_InteractableObject interactableObject;

    private bool closingByOther;

    private void Awake()
    {
        joint = GetComponent<HingeJoint>();
        interactableObject = GetComponent<VRTK_InteractableObject>();

        interactableObject.disableWhenIdle = false;
        joint.useSpring = false;
        
        closingByOther = false;
    }

    private void Update()
    {
        if (closingByOther)
            return;
        
        joint.useSpring = Mathf.Abs(joint.angle - joint.limits.min) < minAngleToAutoClose;
    }

    public void CloseDoor(Action callback)
    {
        StartCoroutine(ClosingDoor(callback));
    }

    private IEnumerator ClosingDoor(Action callback)
    {
        interactableObject.enabled = false;
        joint.useSpring = true;

        closingByOther = true;
        
        while (Mathf.Abs(joint.angle - joint.limits.min) > minAngleDifferenceToStopClosing)
        {
            yield return new WaitForSeconds(.25f);
        }
        
        interactableObject.enabled = true;
        joint.useSpring = false;
        
        closingByOther = false;
        
        callback.Invoke();
    }
}
