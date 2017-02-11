using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;


public class Teleporter : MonoBehaviour {

    bool isInTeleporter;

    void Start()
    {
        isInTeleporter = false;
    }

    void Update()
    {
        if (isInTeleporter && LevelFader.Instance.canProceedToLoadingScreen)
            EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().buildIndex +1,UnityEngine.SceneManagement.LoadSceneMode.Single);
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
