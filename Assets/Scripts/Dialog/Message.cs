using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message : MonoBehaviour {

	[SerializeField]
	private int index;
	[SerializeField]
	private bool canRepeat;

	[SerializeField] private bool pauseGameOnShowDialog = true;
	[SerializeField] private bool autoClose = false;


	private bool isCleared = false;
	
	// Use this for initialization


	void Start()
	{
		isCleared = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{

		if(!isCleared)
		{
			DialogManager.dialogManager.LoadConversationByIndex(index);
			DialogManager.dialogManager.SetAutoClose(autoClose);


			if (pauseGameOnShowDialog)
			{
				BlurCameraOverTime.Instance.BlurScreen();
				Chronos.PauseTime(0.05F);
				DialogManager.dialogManager.SetCallbackFunc(OnMessageEnd);
				Chronos.LateExecute(DialogManager.dialogManager.ShowDialogBox, 0.6F);
			}
			else
			{
				DialogManager.dialogManager.SetCallbackFunc(DefaultBehavior);
				DialogManager.dialogManager.ShowDialogBox();
			}

			isCleared = !canRepeat;
			
		}
	}

	void OnMessageEnd()
	{
		BlurCameraOverTime.Instance.UnblurScreen();
		Chronos.ResumeTime(0.05F);
	}

	void DefaultBehavior()
	{
		//Do nothing
	}

}
