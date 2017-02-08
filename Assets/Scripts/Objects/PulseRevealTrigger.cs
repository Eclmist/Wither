using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseRevealTrigger : MonoBehaviour, IInteractable {

    private bool pulseStarted;

    [Header("Glow")]
    [SerializeField] private float opacity;
    [SerializeField] private float maxGlowOpacity;
    [SerializeField]
    [Range(0,1)]
    private float glowRate;

    [SerializeField]
    private AnimationCurve opacityCurve;


    private Renderer renderer;
    private float additionalOpacity;
    void IInteractable.Interact()
    {
    }


    IEnumerator DoPulse()
    {
        pulseStarted = true;

        additionalOpacity = 0;

        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        float opacityFromCurve = 0;

        while (opacityFromCurve < 1)
        {
            additionalOpacity = opacityCurve.Evaluate(opacityFromCurve);
            additionalOpacity *= maxGlowOpacity;
            opacityFromCurve += glowRate / 10;

            yield return wait;
        }


        additionalOpacity = 0;

        pulseStarted = false;

    }


    // Use this for initialization
    void Start () {
        renderer = GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {
        float tempOpacity = Mathf.Clamp(opacity + additionalOpacity, 0, maxGlowOpacity);

        renderer.material.SetFloat("_Opacity", tempOpacity);
    }
    

    void IInteractable.Pulse()
    {
        if (!pulseStarted)
        {
            StartCoroutine("DoPulse");
        }
    }
}
