using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequest{

    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart,Vector3 pathEnd, Action<Vector3[],bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
	
}
