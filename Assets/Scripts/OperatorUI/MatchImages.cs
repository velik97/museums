using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchImages : MonoBehaviour {

	public Image loadingImage;
	public Image matchImage;
	public Image notMatchImage;
	
	public void SetLoading()
	{
		loadingImage.gameObject.SetActive(true);
		matchImage.gameObject.SetActive(false);
		notMatchImage.gameObject.SetActive(false);
  	}
    
	public void SetMatch()
	{
		loadingImage.gameObject.SetActive(false);
		matchImage.gameObject.SetActive(true);
		notMatchImage.gameObject.SetActive(false);        
	}
    
	public void SetNotMatch()
	{
		loadingImage.gameObject.SetActive(false);
		matchImage.gameObject.SetActive(false);
		notMatchImage.gameObject.SetActive(true);
    }

	public void SetEmpty()
	{
		loadingImage.gameObject.SetActive(false);
		matchImage.gameObject.SetActive(false);
		notMatchImage.gameObject.SetActive(false);       
	}
}
