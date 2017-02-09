using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunnable : MonoBehaviour {

    private Animator animator;

	void Awake()
    {
        animator = GetComponent<Animator>();

    }


    public void Stun(float duration)
    {
        if (animator != null)
            StartCoroutine(ApplyStun(duration));
        else
            Debug.Log("Object does not have an animator...");
    }

    IEnumerator ApplyStun(float duration)
    {
        animator.enabled = false;
        yield return new WaitForSeconds(duration);
        animator.enabled = true;

    }





}
