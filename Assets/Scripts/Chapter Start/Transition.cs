using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{

	public static int level;

	[SerializeField]
	private string message; // Message to diplay
	[SerializeField]
	private Text text;      // The UI text component to display message
	[SerializeField]
	private Text header;    // Chapter header
	[SerializeField]
	private string headerText;  // Text for header
	[SerializeField]
	private float messageFadeInTime;   // Time for message to fade in
	[SerializeField]
	private float headerFadeInTime;    // Time for header to fade in
	[SerializeField]
	private float stayTime;             // Duration of Header
	[SerializeField]
	private float delayBeforeNextScene; // Duration before next scene loads

	private bool isMessageShown;
	private GameObject spinner;

	void Awake()
	{
		level = 0;
	}

	// Use this for initialization
	IEnumerator Start ()
	{
		level ++;
		UpdateText(level);
		// Get the spinner sprite and hide it
		spinner = GameObject.Find("Spinner");
		spinner.SetActive(true);
		isMessageShown = false;
		// Initialize message and flower frame to be invisible
		text.canvasRenderer.SetAlpha(0);
		header.canvasRenderer.SetAlpha(0);

		DisplayHeader();
		yield return new WaitForSeconds(stayTime);
		FadeIn();
		yield return new WaitForSeconds(delayBeforeNextScene);
		isMessageShown = true;
	}

	void Update()
	{
		if (LoadScene.currentLoadProgress >= 0.9F && isMessageShown)
		{
			spinner.SetActive(false);
		}

	}

	void FadeIn()
	{
		text.CrossFadeAlpha(1, messageFadeInTime, false);
	}

	void DisplayHeader()
	{
		header.CrossFadeAlpha(1, headerFadeInTime, false);
	}

	public void UpdateText(int level)
	{
		header.text = "Chapter " + level;

		switch (level)
		{
			case 1:
				text.text = "\"My Name is Ivy\"";
				break;
			case 2:
				text.text = "\"She brought colors to my life\"";
				break;
			case 3:
				text.text = "\"Don't know la\"";
				break;
			case 4:
				break;
		}
	}


}
