using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    bool isInTeleporter;

    void Start()
    {
        isInTeleporter = false;
    }

    void Update()
    {
        if (isInTeleporter && LevelFader.Instance.canProceedToLoadingScreen)
            LevelLoader.LoadNextLevel();
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
