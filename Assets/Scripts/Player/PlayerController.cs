using UnityEngine;
using System.Collections;

class PlayerController : MonoBehaviour
{
	public static PlayerController Instance;

	[Range(0,30)]
	[SerializeField]
	private float moveSpeedMultiplier;

	private Rigidbody rigidBody;
	private Animator animator;
	private bool isMoving;
	public bool isDead;

	[SerializeField]
	private string shaderName;

	// Input axis
	private float horizontal;
	private float vertical;

	public void ApplyStun(float duration)
	{
		StartCoroutine(StatusEffect(duration));
	}

	public IEnumerator StatusEffect(float duration)
	{
		animator.enabled = false;
		yield return new WaitForSeconds(duration);
		animator.enabled = true;
	}

	// Use this for initialization
	void Start ()
	{
		Instance = this;
		rigidBody = GetComponent<Rigidbody>();
		animator = GetComponentInChildren<Animator>();
		isMoving = false;

	}
	
	// Update is called once per frame
	void Update ()
	{
		
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		HandleAnimations();

	}

	void FixedUpdate()
	{
		if(!isDead)
			HandleMovement(horizontal, vertical);
	}


	// Handle animations
	void HandleAnimations()
	{
		animator.SetBool("isMoving", isMoving);
		animator.SetBool("isDead", isDead);
	}


	// Handles all player movement
	void HandleMovement(float horizontal, float vertical)
	{

		// Player faces direction of movement
		Vector3 direction = new Vector3(horizontal, 0,vertical);
		transform.LookAt(Vector3.Lerp(transform.position, transform.position + direction * 10, Time.deltaTime));

		if (vertical!=0 || horizontal != 0)
		{
			isMoving = true;
			Vector3 hz = new Vector3(horizontal, 0, vertical).normalized * moveSpeedMultiplier;
			rigidBody.velocity = new Vector3(hz.x, rigidBody.velocity.y, hz.z);
			
			
		}

		if (horizontal == 0 && vertical == 0)
		{
			isMoving = false;
			rigidBody.velocity = new Vector3(0,
				Mathf.Clamp(rigidBody.velocity.y, float.MinValue, 0),
				0);

		} 
	}

	public float GetSpeed()
	{
		return moveSpeedMultiplier;
	}

    void OnDisable()
    {
        horizontal = 0;
        vertical = 0;
        animator.enabled = false;
    }

	

}

