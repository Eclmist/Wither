using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class IvyController : MonoBehaviour
{
    [SerializeField]
    [Range(0,1)]
    private float speed;

    [SerializeField]
    [Range(0, 10)]
    private float floatingHeight;

    [SerializeField]
    private LayerMask ignoreMask = 1 << 9;


    Vector3 targetPosition;

    // Use this for initialization
    void Start ()
    {
        targetPosition = transform.position;
    }


    // Update is called once per frame
    void FixedUpdate ()
    {
        transform.LookAt(targetPosition);

        if (Input.GetAxis("Fire1") > 0)
        {
            Ray targetPosRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;


            if (Physics.Raycast(targetPosRay, out hit, float.MaxValue, ~ignoreMask))
            {
                RaycastHit obstacleHit;

                if (Physics.Linecast(transform.position, hit.point, out obstacleHit))
                {
                    targetPosition = obstacleHit.point;

                    Vector3 dir = targetPosition - transform.position;
                    dir = dir.normalized;

                    Vector3 newTarget = targetPosition - dir * 0.2F;

                    RaycastHit floorPoint;

                    if (Physics.Raycast(newTarget, Vector3.down, out floorPoint))
                    {
                        newTarget.y = floorPoint.point.y + floatingHeight;
                    }
                    else
                    {
                        Debug.Log("GG no floor");
                    }

                    targetPosition = newTarget;

                    //targetPosition.y += floatingHeight;
                }
                else
                {
                    targetPosition = hit.point;
                    targetPosition.y += floatingHeight;
                }

            }

        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, speed / 10);
    }
}
