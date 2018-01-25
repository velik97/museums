using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSwitch : MonoBehaviour
{
	public List<GameObject> portals;
	public int initial;

	private int last;

	private void Awake()
	{
		SwitchPortal(initial);

		last = initial;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			last++;
			last %= 4;
			SwitchPortal(last);
		}
	}

	public void SwitchPortal (int index)
	{
		for (var i = 0; i < portals.Count; i++)
		{
			portals[i].SetActive(index == i);
		}
	}
}
