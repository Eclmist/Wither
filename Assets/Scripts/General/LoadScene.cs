using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public static LoadScene Instance;

    private AsyncOperation ao;
    private bool coroutineStarted = false;

	public static float currentLoadProgress;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

	    if (Instance != null)
		    Destroy(this);
		else
			Instance = this;

	}

    //TODO: Samuel: Make some overloads for this function
    public void Load(int index)
    {
        // Load the loading screen scene
        SceneManager.LoadSceneAsync(1);

        if (!coroutineStarted)
            StartCoroutine(LoadLevelAsync(index));
    }

    public void Load(string name)
    {
        // Load the loading screen scene
        SceneManager.LoadSceneAsync(1);

        if (!coroutineStarted)
            StartCoroutine(LoadLevelAsync(name));

    }


    IEnumerator LoadLevelAsync(int index)
    {
        coroutineStarted = true;

        ao = SceneManager.LoadSceneAsync(index);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
	        currentLoadProgress = ao.progress;


			if (ao.progress >= 0.9F)
            {
                if (Input.anyKeyDown)
                {
                    ao.allowSceneActivation = true;
					currentLoadProgress = 1;
				}
			}

            yield return null;
        }

		coroutineStarted = false;
    }

    IEnumerator LoadLevelAsync(string name)
    {
        coroutineStarted = true;

        ao = SceneManager.LoadSceneAsync(name);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
			currentLoadProgress = ao.progress;

			if (ao.progress >= 0.9F)
            {
                if (Input.anyKeyDown)
                {
                    ao.allowSceneActivation = true;
					currentLoadProgress = 1;
				}
			}

            yield return null;
        }

        coroutineStarted = false;
    }


}
