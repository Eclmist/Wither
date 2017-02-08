using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour {

    public void PlayerTrigger(Vector3 position, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius, LayerMask.GetMask("PlayerTrigger"));

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Destroy(hitColliders[i].gameObject);
        }
    }

    public void IvyTrigger(Vector3 position, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius, LayerMask.GetMask("IvyTrigger"));

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Destroy(hitColliders[i].gameObject);
        }
    }
}
