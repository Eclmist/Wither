using UnityEngine;
using System.Collections;

public class PuzzleManager : MonoBehaviour {

    protected GameObject[] obstacles;
    protected Collider colliders;

    void Start()
    {
        if(obstacles == null)
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        //collider = this.gameObject.GetComponent<Collider>();
    }

	void Update ()
    {
        if (obstacles != null)
        {
            PuzzleUpdate();
        }
    }

    protected void PuzzleUpdate()
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null)
            {
                colliders = obstacles[i].GetComponent<Collider>();

                if (colliders.enabled == true)
                {
                    if (obstacles[i].transform.childCount == 0)
                    {
                        Destroy(obstacles[i]);
                    }
                }
            }

        }
    }
}
