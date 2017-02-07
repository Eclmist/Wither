using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This component is the representation of health.
/// It contains draining effects.
/// </summary>
public class HealthEffect : MonoBehaviour {

    public Image healthEmpty;
    public Image healthFilled;
    public Image healthDrained;
    public float difference { get; set; }
    private bool isInstanceRunning = false;

    // Takes in damage/max health because fill amount starts from 0 - 1
    public void ReduceHealth(float amount)
    {
        healthFilled.fillAmount -= amount;
        difference = healthDrained.fillAmount - healthFilled.fillAmount;

        // Ensure only one instance of coroutine is running (infinite loop)
        if (!isInstanceRunning)
            StartCoroutine("SlowReduction");
    }

    // Reduce health gradualy
    IEnumerator SlowReduction()
    {
        isInstanceRunning = true;
        float target = healthFilled.fillAmount;

        while (healthDrained.fillAmount >0)
        {

            if (healthDrained.fillAmount > healthFilled.fillAmount)
            {   
                healthDrained.fillAmount -= 0.01f * difference;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
