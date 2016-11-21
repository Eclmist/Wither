using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoManager : MonoBehaviour {

    public static int PickupCount = 0;
    public static MementoManager Instance;
    public Renderer ren;
    public Renderer backfaceRen;

    private bool shown;

    void Awake()
    {
        Instance = this;
    }

    void Start () {
		
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            if (shown)
            {
                shown = false;
                HideMemento();
            }
        }
	}

    public static void IncrementPickupCount()
    {
        PickupCount++;
    }

    public static void ShowMemento()
    {
        Instance.GetComponent<Camera>().enabled = true;
    }

    public static void HideMemento()
    {
        Chronos.ResumeTime(0.05F);
        BlurCameraOverTime.Instance.UnblurScreen();
        Instance.StartCoroutine(Instance.TriggerMemento(false));
    }

    public IEnumerator TriggerMemento(bool enabled)
    {

        float startingOpacity = 0;
        ren.material.SetFloat("_Opacity", startingOpacity);
        backfaceRen.material.SetFloat("_Opacity", startingOpacity);

        if (!enabled)
        {
            startingOpacity = 0;
            Instance.GetComponent<Camera>().enabled = false;
        }
        else //Enabled
        {
            while (startingOpacity < 1)
            {

                startingOpacity += 0.01F;

                if (startingOpacity >= 1)
                {
                    startingOpacity = 1;
                    Instance.shown = true;
                }

                ren.material.SetFloat("_Opacity", startingOpacity);
                backfaceRen.material.SetFloat("_Opacity", startingOpacity);

                yield return new WaitForSecondsRealtime(0.01F);
            }
        }
    }
}
