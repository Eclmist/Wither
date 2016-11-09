using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform[] target;

    private Vector3 offset;

	// Use this for initialization
	void Start ()
	{
	    offset = - target[0].transform.position + transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    Vector3 targetPos = target[0].position * 0.8F;

	    targetPos += target[1].position*0.2F;


	    transform.position = Vector3.Lerp(transform.position, targetPos + offset, Time.deltaTime);
	}
}
