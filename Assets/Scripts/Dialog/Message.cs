using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message : MonoBehaviour {

    [SerializeField]
    private int index;
    [SerializeField]
    private bool canRepeat;

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
            DialogManager.dialogManager.ShowDialogBox();
            isCleared = !canRepeat;
            
        }
    }
        
        
}
