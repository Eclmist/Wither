using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

	public static DialogManager dialogManager; // Static instance of dialog manager
	private GameObject dialogBox; // The dialog box containing the text
	private string[] lines;     // Split text resource into lines
	private Text currentText; // Text that is currently displayed
	private string currentLine; // The line that is currently being processed
	private GameObject currentCharacter; // Sprite to represent the character that is currently talking
	private int iterator = 0; // To iterate through conversations

	private TextAsset conversations; // Text resource containing all conversation information
	private List<string> retrievedLines; // List to store lines that belong to a particular conversation
	private List<string> characterOrder; // List to store which character is currently talking
	private List<string> messageOrder;  // List to store the messages in order

	[Range(0,0.1F)]
	[SerializeField]
	private float textSpeed;    // Interval between text characters
	private bool isShowing;     // Status of dialog box
	private bool isTyping;      // Check if  a co routine is currently running
	private bool canContinue;   // Check if user clicked

	private Callback onDialogEnd;
	private bool autoCloseDialogBoxOnMessageEnd;
	private bool alreadyClosing;

	private float timeSinceLastLetter = 0;
	private bool allowTypewriterToRun = false;
	void Awake()
	{
		transform.FindChild("Dialog Canvas").gameObject.SetActive(true);

		// Load the text resource
		conversations = Resources.Load("Conversations") as TextAsset;

		if (conversations == null)
			Debug.LogError("Conversations resource missing!");
		else
			lines = conversations.text.Split('\n');

		isTyping = false;
		canContinue = true;
		isShowing = false;
		dialogManager = this;
		dialogBox = GameObject.FindWithTag("DialogBox");
		currentCharacter = GameObject.FindWithTag("Character Sprite");
		currentText = dialogBox.GetComponentInChildren<Text>();
	}

	void Update()
	{
		// Constantly check the status of the dialog box
		dialogBox.SetActive(isShowing);
		currentCharacter.SetActive(isShowing);
		HandleDialogBox();

		timeSinceLastLetter += Chronos.BetaTime;

		if (allowTypewriterToRun && !alreadyClosing)
		{
			isTyping = TypeText();
		}
		else
		{
			isTyping = false;
			currentLineSubstringPos = 0;
		}
	}

	public void ShowDialogBox()
	{
		ToggleDialogBox(true);
	}

	public void HideDialogBox()
	{
		ToggleDialogBox(false);
	}

	// Search through the text resource to find the specified conversation
	// Accessed by outside gameobjects
	public void LoadConversationByIndex(int conversationIndex)
	{
		retrievedLines = new List<string>();
		ClearPreviousConversations();
		bool storeLine = false;

		foreach(string line in lines)
		{
			// ***the following series of operations are in the RIGHT order***

			if (line.Contains("--End") && retrievedLines.Count > 0)
				break;

			// To stop storing lines
			if (line.Contains("--End"))
				storeLine = false;

			// store the current line
			if (storeLine == true)
				retrievedLines.Add(line);

			// This check is at the end to ensure that we only store the line after this header line
			if (line.Contains("--Session " + conversationIndex))
				storeLine = true;
	   
		}

		SortCharacterAndSpeech(retrievedLines);
	}

	public void ToggleDialogBox(bool show)
	{
		isShowing = show;

		if (!isShowing)
		{

			// Reset the iterator
			iterator = 0;
			isTyping = false;
			canContinue = true;
		}
	}

	private int currentLineSubstringPos = 0;

	private bool TypeText()
	{
		if (currentLineSubstringPos <= currentLine.Length)
		{

			if (timeSinceLastLetter > 0.1f - textSpeed)
			{
				timeSinceLastLetter = 0;
				currentText.text = currentLine.Substring(0, currentLineSubstringPos);
				currentLineSubstringPos++;
			}

			return true;
		}

		allowTypewriterToRun = false;
		return false;
	}

	// Handles how messages are being displayed in the dialog box (eg. order of messages etc)
	private void HandleDialogBox()
	{

		// If the dialog box is currently active
		if (isShowing)
		{

			// If there is no co routine AND the user can continue to next line
			if (!isTyping && canContinue)
			{
				isTyping = true;    
				canContinue = false; 
   
				// Start the co routine
				ProceedMessage();
			}

			if (Input.GetKeyDown(KeyCode.Mouse0))
			{

				// If the text is still typing AND the player presses proceed, display the full text
				if (isTyping)
				{
					isTyping = false;
					allowTypewriterToRun = false;
					currentText.text = messageOrder[iterator-1];
					
				}
				else
				{
					canContinue = true;
				}
			}

			// Reached the last line of character speech
			if (iterator >= messageOrder.Count)
			{
				if (!isTyping && !alreadyClosing && autoCloseDialogBoxOnMessageEnd)
				{
					alreadyClosing = true;
					Chronos.LateExecute(HideDialogBox, 0.5f);
				}

				if (canContinue)
				{
					//Call the callback func
					onDialogEnd();
					HideDialogBox();
				}
			}
		}
	}

	
	private void ProceedMessage()
	{
		currentLine = messageOrder[iterator];
		currentCharacter.GetComponent<Image>().sprite = Resources.Load<Sprite>("Characters/" + characterOrder[iterator]) as Sprite;


		allowTypewriterToRun = true;
		iterator++;

	}

	// Seperate character name and speech
	// Core function to set up conversation
	private void SortCharacterAndSpeech(List<string> retrievedConversation)
	{
		characterOrder = new List<string>();
		messageOrder = new List<string>();

		foreach (string m in retrievedConversation)
		{
			string[] temp = m.Split('|');
			characterOrder.Add(temp[0]);
			messageOrder.Add(temp[1]);
		}
	}

	// Removes previously added items in all lists
	private void ClearPreviousConversations()
	{
		if (retrievedLines != null)
			retrievedLines.Clear();

		if (characterOrder != null)
			characterOrder.Clear();

		if (messageOrder != null)
			messageOrder.Clear();
	}

	public void SetCallbackFunc(Callback function)
	{
		onDialogEnd = function;
	}

	public void SetAutoClose(bool e)
	{
		autoCloseDialogBoxOnMessageEnd = e;
		alreadyClosing = false;
	}

	public delegate void Callback();
}
