using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPickUp : MonoBehaviour {

	public int id;				// -1 is trash
	
	private static Dictionary<int, FinalPickUp> pickUpByIndex; 

	[HideInInspector]
	public ArtefactInfo artefactInfo;

	public static Dictionary<int, FinalPickUp> PickUpByIndex
	{
		get { return pickUpByIndex; }
	}

	private void Awake()
	{	
		if (pickUpByIndex != null && pickUpByIndex.Keys.Count != 0)
			pickUpByIndex.Clear();
		
		WaveGameSetter.SubscribeOnWave(SetPickUpsDictionary, 0);
		
		// TODO prototype line ===================================
		WaveGameSetter.SubscribeOnWave(delegate
		{
			if (GetComponent<PrototypeMuseumMaterials>() != null)
				GetComponent<PrototypeMuseumMaterials>().SetMaterial(artefactInfo.museum);
		}, 2);
		// =======================================================
	}

	private void SetPickUpsDictionary()
	{
		artefactInfo = GameInfo.Instance.ArtefactById(id);
		
		if (pickUpByIndex == null)
			pickUpByIndex = new Dictionary<int, FinalPickUp>();
			
		if (!pickUpByIndex.ContainsKey(id))	
			pickUpByIndex.Add(id, this);
		else
			Debug.LogError("[Final Pick Up] More than one final pickup with id: " + id);
		
	}

	public static void ResetList()
	{
		print("reset pick ups");		
		pickUpByIndex.Clear();
	}
}
