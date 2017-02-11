using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SineFadeText : MonoBehaviour
{

	public Text text;
	public float speedMultiplier;
	private float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer += Chronos.BetaTime * speedMultiplier;
		text.color = new Color(1, 1, 1, (Mathf.Abs(Mathf.Sin(timer)) + 0.2F));
	}
}
