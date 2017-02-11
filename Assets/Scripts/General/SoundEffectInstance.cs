using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectInstance : MonoBehaviour {

    AudioSource source;

    void Awake()
    {
        source = this.gameObject.AddComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
        if(!source.isPlaying)
        {
            Destroy(gameObject);
        }
        
	}



}
