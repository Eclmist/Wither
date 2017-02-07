using UnityEngine;
using System.Collections;
using System;

public class Distance : MonoBehaviour, IComparable<Distance>{

    public GameObject origin;

    public int CompareTo(Distance other)
    {
        float otherDistance = Vector3.Distance(other.transform.position, origin.transform.position);
        float thisDistance = Vector3.Distance(transform.position, origin.transform.position);
        if (thisDistance > otherDistance)
            return 1;
        else if (thisDistance < otherDistance)
            return -1;
        else
            return 0;
    }
}
