using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;

    void Awake()
    {
        Instance = this;
    }
	
	public void PlaySound(AudioClip clip,GameObject reference)
    {
        // Check if clip exist
        if (clip != null)
        {
            if (reference.GetComponent<AudioSource>() != null)
                reference.GetComponent<AudioSource>().PlayOneShot(clip);
            else
                Debug.Log("audio source for " + reference.name + "not found...");
        }
        else
            Debug.Log(clip.name + " clip not found...");
        
    }

    public void PlaySound(AudioClip clip, GameObject reference,float volume)
    {
        // Check if clip exist
        if (clip != null)
        {
            if (reference.GetComponent<AudioSource>() != null)
                reference.GetComponent<AudioSource>().PlayOneShot(clip, Mathf.Clamp01(volume));
            else
                Debug.Log("audio source for " + reference.name + "not found...");
        }
        else
            Debug.Log(clip.name + " clip not found...");

    }

    public void PlaySoundAt(AudioClip clip, Vector3 target, GameObject source)
    {
        if (clip != null)
        {
            GameObject audioHolder = new GameObject();
            SoundEffectInstance sfxInstance = audioHolder.AddComponent<SoundEffectInstance>();
            audioHolder.transform.parent = source.transform;
            audioHolder.transform.position = target;

            sfxInstance.GetComponent<AudioSource>().PlayOneShot(clip);
         }
    }
}
