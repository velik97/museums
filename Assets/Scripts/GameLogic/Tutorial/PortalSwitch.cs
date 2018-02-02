using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSwitch : MonoSingleton<PortalSwitch>
{
    public List<GameObject> portals;

    [Tooltip("Walls near portals surface should be disabled, when portals are arctive")]
    public List<GameObject> underPortalWalls;

    public int initial;

    private int currentIndex;

    private void Awake()
    {
        SwitchPortal(initial);

        currentIndex = initial;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPortal();
        }
    }

    public void SwitchPortal()
    {
        currentIndex++;
        currentIndex %= portals.Count;
        SwitchPortal(currentIndex);
    }

    public void SwitchPortal(int index)
    {
        bool atLeastOneIsActive = false;

        Portal portal;
//        portal.

        for (var i = 0; i < portals.Count; i++)
        {
            portals[i].SetActive(index == i);
            if (index == i)
                atLeastOneIsActive = true;
        }
        
        for (var i = 0; i < underPortalWalls.Count; i++)
        {
            underPortalWalls[i].SetActive(!atLeastOneIsActive);
        }

        currentIndex = index;
    }
}