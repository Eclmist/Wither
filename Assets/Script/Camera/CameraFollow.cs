using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 offset;

	// Use this for initialization
	void Start ()
	{
	    offset = - target.transform.position + transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, Time.deltaTime);
	}
}
