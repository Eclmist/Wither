using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField]
    private float rate; 

	// Update is called once per frame
	void Update ()
    {
        // Lazy to flip image. Just gonna rotate it the other way....
        transform.Rotate(new Vector3(0, 0, 1), -rate);
	}
}
