using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeMuseumMaterials : MonoBehaviour
{
	public Material[] materials;

	public List<MeshRenderer> meshRenderers;

	private void Awake()
	{
		if (meshRenderers == null || meshRenderers.Count == 0)
		{
			meshRenderers = new List<MeshRenderer>();

			MeshRenderer myMesh = GetComponent<MeshRenderer>();
			
			if (myMesh != null)
				meshRenderers.Add(myMesh);
			
			meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
		}
		
		if (materials.Length != 9)
			Debug.LogError("Materials count doesn't match count of museums");
	}

	public void SetMaterial(Museum museum)
	{
		SetMaterial((int)museum);
	}
	
	public void SetMaterial(int museumIndex)
	{
		foreach (var mr in meshRenderers)
		{
			mr.material = materials[museumIndex];
		}
	}
	
}
