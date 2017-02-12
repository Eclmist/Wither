using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    bool isInTeleporter;
	public string targetLevelName;

    void Start()
    {
        isInTeleporter = false;
    }

    void Update()
    {
        if (isInTeleporter && LevelFader.Instance.canProceedToLoadingScreen)
            LoadScene.Instance.Load("targetLevelName");
    }
        


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInTeleporter = true;
            other.GetComponent<PlayerController>().enabled = false;
            LevelFader.Instance.FadeOut();
        }

    }

  


}
