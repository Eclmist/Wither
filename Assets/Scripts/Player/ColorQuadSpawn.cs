using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class ColorQuadSpawn : MonoBehaviour {

    private ParticleSystem.EmissionModule em;
    private ParticleSystem.MainModule main;
    public ParticleSystem colorSpawn;
    private Rigidbody rigidBody;
    private PlayerController playerController;

    // Use this for initialization
    void Start () {
        em = colorSpawn.emission;
        main = colorSpawn.main;
		rigidBody = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
    }
    RaycastHit hit;

    // Update is called once per frame
    void FixedUpdate()
    {

        em.enabled = (rigidBody.velocity.magnitude > 0.1F);
        em.rateOverTime = playerController.GetSpeed()*2.5F;

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            colorSpawn.transform.position = hit.point;

			colorSpawn.transform.up = hit.normal;

			//    //Quaternion newRot = new Quaternion();
			//    //newRot.SetLookRotation(hit.normal, colorSpawn.transform.up);

			//    //colorSpawn.transform.rotation = newRot;

			//}

		}

	}

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hit.point, 1);
    }

}
