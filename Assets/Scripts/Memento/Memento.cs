using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memento : MonoBehaviour
{
    public Texture currentTex;

    void OnTriggerEnter(Collider other)
    {
        BlurCameraOverTime.Instance.BlurScreen();
        Chronos.PauseTime(0.05F);
        Chronos.LateExecute(MementoManager.ShowMemento, 0.6F);
        MementoManager.IncrementPickupCount();
        MementoManager.Instance.StartCoroutine(MementoManager.Instance.TriggerMemento(true));
        Destroy(gameObject);
    }
}
