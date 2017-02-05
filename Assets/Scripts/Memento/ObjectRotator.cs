using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour
{
    private Quaternion targetRotation;
    private bool isRotating;

    private Vector3 mouseDelta;
    private Vector3 mouseStartPos;
    private Quaternion startRotation;
    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
            mouseStartPos = Input.mousePosition;
            startRotation = transform.rotation;
            targetRotation = transform.rotation;
        }
        else if (!Input.GetMouseButton(0))
        {
            isRotating = false;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Chronos.BetaTime * 5);
        }

        if (isRotating)
        {
            mouseDelta = Input.mousePosition - mouseStartPos;
            mouseDelta.x /= Screen.width;
            mouseDelta.y /= Screen.height;

            Quaternion yRot = Quaternion.AngleAxis(-mouseDelta.x * 140, new Vector3(0, 1, 0));

            Quaternion xRot = Quaternion.AngleAxis(mouseDelta.y * 140, new Vector3(1,0,0));

            targetRotation = xRot  * yRot * startRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Chronos.BetaTime * 10);

        }
    }
}