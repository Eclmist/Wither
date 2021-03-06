﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelFader : MonoBehaviour {
    
    public static LevelFader Instance;

    public float fadeInDuration;
    public float fadeOutDuration;
    public Image blackOverlay;
    [HideInInspector]
    public bool canProceedToLoadingScreen;

	void Awake()
	{
		Instance = this;
		canProceedToLoadingScreen = false;
	}

	void Start()
	{
		FadeIn();
	}
		

	public void FadeIn()
	{
		blackOverlay.CrossFadeAlpha(0,fadeInDuration,true);
	}

	public void FadeOut()
	{
		blackOverlay.CrossFadeAlpha(1, fadeOutDuration, true);
		StartCoroutine(WaitPermission(fadeOutDuration));
	}

	IEnumerator WaitPermission(float t)
	{

        yield return new WaitForSeconds(t);
        canProceedToLoadingScreen = true;
    }

    //public void FadeOutAndLoadScene(string targetLevelName)
    //{
    //    blackOverlay.CrossFadeAlpha(1, fadeOutDuration, true);
    //    StartCoroutine(WaitPermission(fadeOutDuration));

    //    if (canProceedToLoadingScreen)
    //        LoadScene.Instance.Load(targetLevelName);
                  
    //}
        
}
