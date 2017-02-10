using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avoidance : MonoBehaviour {

    public float minimumDistToAvoid;
    public LayerMask avoidanceIgnoreMask;
    public float force;
    public AnimationCurve turnRateOverAngle;

    private GameObject player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    public Vector3 AvoidObstacles()
    {
        RaycastHit Hit;
        //Check that the vehicle hit with the obstacles within it's minimum distance to avoid
        Vector3 right45 = (transform.forward + transform.right).normalized;
        Vector3 left45 = (transform.forward - transform.right).normalized;

        if (Physics.Raycast(transform.position, right45, out Hit,
            minimumDistToAvoid, ~avoidanceIgnoreMask))
        {
            // 0 if near, 1 if far
            float distanceExp = Vector3.Distance(Hit.point, transform.position) / minimumDistToAvoid;
            // 5 if near, 0 if far
            distanceExp = 5 - distanceExp * 5;

            return transform.forward - transform.right * force * distanceExp;
        }
        else if (Physics.Raycast(transform.position, left45, out Hit,
            minimumDistToAvoid, ~avoidanceIgnoreMask))
        {

            // 0 if near, 1 if far
            float distanceExp = Vector3.Distance(Hit.point, transform.position) / minimumDistToAvoid;
            // 5 if near, 0 if far
            distanceExp = 5 - distanceExp * 5;

            return transform.forward + transform.right * force * distanceExp;
        }

        else
        {
            //Debug.Log("Turned towards player");
            Debug.DrawLine(transform.position, player.transform.position, Color.blue);
            return player.transform.position - transform.position;
        }
    }


    public void LookAtPlayer()
    {

        Vector3 tempTarget = player.transform.position;

        float distance = (tempTarget - transform.position).magnitude / 60;
        float angle = Vector3.Angle(tempTarget - transform.position, transform.forward) / 180;
        float rotationSpeed = turnRateOverAngle.Evaluate(angle);


        Vector3 lookAtTarget = AvoidObstacles();
        lookAtTarget.y = 0; //Force no y change;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(lookAtTarget, Vector3.up),
            rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * minimumDistToAvoid / 3);
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward + transform.right) * minimumDistToAvoid);
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward - transform.right) * minimumDistToAvoid);
    }

}
