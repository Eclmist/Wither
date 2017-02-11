using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class Transition : MonoBehaviour
{

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
    private AsyncOperation operation;
    
    public void SetMessage(string message)
    {
        text.text = message;
    }

    public void SetHeaderText(string headerText)
    {
        header.text = headerText;
    }
        
   
    // Use this for initialization
    IEnumerator Start ()
    {
        //operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1,LoadSceneMode.Single);
        operation = EditorSceneManager.LoadSceneAsync(EditorSceneManager.GetActiveScene().buildIndex + 1, UnityEngine.SceneManagement.LoadSceneMode.Single);
        operation.allowSceneActivation = false;   
        
   
        // Get the spinner sprite and hide it
        spinner = GameObject.Find("Spinner");
        spinner.SetActive(false);
        isMessageShown = false;
        // Initialize message and flower frame to be invisible
        text.text = message;
        text.canvasRenderer.SetAlpha(0);
        header.text = headerText;
        header.canvasRenderer.SetAlpha(0);


        DisplayHeader();
        yield return new WaitForSeconds(stayTime);
        FadeIn();
        yield return new WaitForSeconds(delayBeforeNextScene);
        isMessageShown = true;
  
	}

    void Update()
    {

        // If the message has been shown and the scene is still loading
        if(operation.isDone == false && isMessageShown)
            spinner.SetActive(true);

        if (operation.progress >= 0.89F && isMessageShown)
        {
            spinner.SetActive(false);
            operation.allowSceneActivation = true;
           
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


}
