using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

	public string targetLevelName;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().enabled = false;
	        FadeToBlack.Instance.QueueNextOpacity(0);
			LoadScene.Instance.Load("targetLevelName");
		}

	}

  


}
