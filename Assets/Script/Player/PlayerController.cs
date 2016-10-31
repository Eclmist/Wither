using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Range(1,20)]
    public float speed;
    public ParticleSystem colorSpawn;
    private Rigidbody rigidBody;
    private ParticleSystem.EmissionModule em;
    private Vector3 defaultPos;
    // Use this for initialization
    void Start ()
	{
	    rigidBody = GetComponent<Rigidbody>();
        em = colorSpawn.emission;
        defaultPos = colorSpawn.transform.localPosition;
	}
    RaycastHit hit;

    // Update is called once per frame
    void FixedUpdate () {
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

	    rigidBody.velocity = new Vector3(x, rigidBody.velocity.y, z);

        transform.LookAt(transform.position + new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z));

	    em.enabled = (rigidBody.velocity.magnitude > 0.1F);
	    em.rateOverTime = speed*2.5F;

	    if (Physics.Raycast(transform.position, Vector3.down, out hit))
	    {
	        colorSpawn.transform.position =
	            new Vector3(transform.position.x,
                hit.point.y - defaultPos.y - 2,
                transform.position.z);
	    }

    }

}
