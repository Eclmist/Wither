using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
	public CanvasGroup pauseUI;

	private bool isPaused;
	private bool transitioning;


	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape) && !transitioning)
		{
			transitioning = true;
			TogglePause();
		}

		FadePauseUI(isPaused);
	}

	void TogglePause()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			BlurCameraOverTime.Instance.BlurScreen();
			Chronos.PauseTime(0.05F);
		}
		else
		{
			BlurCameraOverTime.Instance.UnblurScreen();
			Chronos.ResumeTime(0.05F);
		}

		pauseUI.interactable = isPaused;
	}

	void FadePauseUI(bool visible)
	{
		float target = visible ? 1 : 0;
		float difference = target - pauseUI.alpha;
		pauseUI.alpha += difference*Chronos.BetaTime * 5;

		if (Mathf.Abs(difference) < 0.01F)
			transitioning = false;
	}
}
