//Not to be confused with the actual overpriced plugin on the asset store

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chronos : MonoBehaviour
{
    public static Chronos Instance;

    private static float realtimeLastFrame;

    public static float BetaTime
    {
        get { return Time.realtimeSinceStartup - realtimeLastFrame; }
    }

    private static bool timeWarping;

    protected void Awake()
    {
        Instance = this;
    }

    private void ModifyTime(float speed, float target)
    {
        if (timeWarping)
        {
            StopCoroutine("LerpTimeCoroutine");
        }

        StartCoroutine(LerpTimeCoroutine(speed, target));
    }

    private void ExecuteCallbackWithDelay(Callback callbackFunc, float time)
    {
        StartCoroutine(LateExecuteCoroutine(callbackFunc, time));
    }

    public static void PauseTime(float speed)
    {
        Instance.ModifyTime(speed, 0);
    }

    public static void ResumeTime(float speed)
    {
        Instance.ModifyTime(speed, 1);
    }

    public delegate void Callback();
    public static void LateExecute(Callback callbackFunc, float time)
    {
        Instance.ExecuteCallbackWithDelay(callbackFunc, time);
    }

    IEnumerator LateExecuteCoroutine(Callback func, float time)
    {
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < time)
        {
            yield return 0;
        }

        func();
    }

    IEnumerator LerpTimeCoroutine(float speed, float target)
    {
        timeWarping = true;

        float factor = 0;
        float currentTimeScale = Time.timeScale;

        while (factor < 1)
        {
            factor += speed;

            Time.timeScale = Mathf.SmoothStep(currentTimeScale, target, factor);
            Time.fixedDeltaTime = 0.02F * Time.timeScale;

            yield return new WaitForFixedUpdate();
        }

        timeWarping = false;
    }

    public void LateUpdate()
    {
        realtimeLastFrame = Time.realtimeSinceStartup;
    }
}
