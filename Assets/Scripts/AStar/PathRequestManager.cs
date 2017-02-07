using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour {

    // Queue to process path requests
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();

    // Stores the current path request
    PathRequest currentRequest;

    public static PathRequestManager instance;
    PathFinder pathFinder;
    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathFinder = GetComponent<PathFinder>();
    }


    // Need to call this every set number of frames (expensive calculation if done per frame)
    public static void RequestPath(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
    {
        // Create a new path request
        PathRequest request = new PathRequest(start,end,callback);
        // Add the request into the request queue
        instance.pathRequestQueue.Enqueue(request);
        instance.TryProcessNext();

    }

    void TryProcessNext()
    {
        // Check if instance is currently processing a path, if not go to next request
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            // Get first request in queue
            currentRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinder.StartFindingPath(currentRequest.pathStart,currentRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool isFound)
    {
        currentRequest.callback(path, isFound);
        isProcessingPath = false;
        TryProcessNext();
    }


	
}
