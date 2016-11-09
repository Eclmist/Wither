using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNearbyEnemies : MonoBehaviour
{

    [SerializeField] [Range(0, 10)] private float range;
    [SerializeField] [Range(0, 10)] private float damage;
    [SerializeField] [Range(0, 10)] private float frequency;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

	    Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemy"));

	    if (nearbyEnemies.Length == 0)
	        return;

	    GameObject target = nearbyEnemies[Random.Range(0, nearbyEnemies.Length)].gameObject;

        //target.DoDamage();
    }
}
