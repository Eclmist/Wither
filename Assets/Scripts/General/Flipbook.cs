using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flipbook : MonoBehaviour
{

	public Sprite[] images;
	public Image canvasImage;

	public float flipTime;
	private float timePassSinceLastFlip;

	private int counter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update()
	{
		timePassSinceLastFlip += Chronos.BetaTime;
		if (timePassSinceLastFlip > flipTime)
		{
			timePassSinceLastFlip = 0;
			counter ++;
			canvasImage.sprite = images[counter%3];
		}
	}
}
