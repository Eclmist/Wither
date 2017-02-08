using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PathAgent : MonoBehaviour
{

    public Transform player;
    float speed = 0.1f;
    Vector3[] path;
    int targetIndex;
    int frame;

    private Rigidbody rb;

    private Vector3 prevPosition;

    public void StopAllInstances()
    {
        StopAllCoroutines();
    }

    public void RequestPathInstantly()
    {
        PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);
    }

    // Use this for initialization
    void Start()
    {

        prevPosition = player.position;
        //PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);

    }

    // Toggle this in a state
    IEnumerator CheckForNew()
    {  
         yield return new WaitForEndOfFrame();
         if (Vector3.Distance(player.position, prevPosition) > 1f)
         {
             PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);
             prevPosition = player.position;
         }
        
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }


    void Update()
    {

        frame++;
        if (frame >= 11)
        {
            StartCoroutine("CheckForNew");
            frame = 0;
        }
     
    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && newPath.Length > 0)
        {
            targetIndex = 0;
            path = newPath;
            StopAllCoroutines();
            StartCoroutine("FollowPath");
        }
    }

    // Follow through wayPoints (position of the nodes)
    IEnumerator FollowPath()
    {

        Vector3 currentWayPoint = path[0];

        while (true)
        {
            Vector3 myVector = new Vector3(transform.position.x, currentWayPoint.y, transform.position.z);

            if (Vector3.Distance(myVector, currentWayPoint) <= 1)
            {
                targetIndex++;

                // If finished path
                if (targetIndex >= path.Length)
                {
                    break;
                }


                currentWayPoint = path[targetIndex];
            }

            //transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, speed);
            //Quaternion rot = Quaternion.LookRotation(currentWayPoint - transform.position);
            Vector3 tempV = currentWayPoint - transform.position;
            tempV.y = transform.rotation.y;
            Quaternion rot = Quaternion.LookRotation(tempV);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10.0f * Time.deltaTime);
            transform.position += transform.forward * 0.1f;
            //Debug.Log("moving");
            yield return new WaitForEndOfFrame();


        } // end while


    }




    void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

}
