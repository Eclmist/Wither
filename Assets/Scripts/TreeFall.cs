using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFall : MonoBehaviour {

    public void CollapseTree()
    {
        transform.localEulerAngles = new Vector3(0, -90, 0);
    }

}
