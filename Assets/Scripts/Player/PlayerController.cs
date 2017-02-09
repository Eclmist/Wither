using UnityEngine;
using System.Collections;

class PlayerController : MonoBehaviour,IDamagable
{
	public static PlayerController Instance;

    [Range(0,30)]
    [SerializeField]
    private float moveSpeedMultiplier;

    private Rigidbody rigidBody;
    private Animator animator;
    private bool isMoving;
    private float health;
    private bool isDead;

    // Variables for overlap sphere detection
    [Range(1,20)]
    [SerializeField]
    private float sphereRadius;
    private Collider[] hitColliders;
    // Indicates if there is an object near enough that can be interacted with
    private bool canInteract;

    [SerializeField]
    private string shaderName;

    // Input axis
    private float horizontal;
    private float vertical;

	public bool IsDead
	{
		get { return isDead; }
		set { this.isDead = value; }
	}

	public float Health
    {
        get { return this.health; }
        set { this.health = value; }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

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
		health = 100;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        isMoving = false;

	}
	
	// Update is called once per frame
	void Update ()
    {
        
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        HandleInput();
        HandleAnimations();

        isDead = (health <= 0);

        hitColliders = Physics.OverlapSphere(this.transform.position,sphereRadius,1);
        CheckForInteractableObjects();

    }

    void FixedUpdate()
    {
        if(!isDead)
            HandleMovement(horizontal, vertical);
    }

    // Handles input from keyboard or mouse
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
            InteractWithObject(GetObjectWithinRange());

     
        

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

    // Returns the nearest INTERACTABLE object within player range
    GameObject GetObjectWithinRange()
    {
   
        GameObject nearestObject = null;
        float nearestDist = float.MaxValue;
        float currentDist;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            // Calculate current distance between object and player
            currentDist = Vector3.Distance(this.transform.position, hitColliders[i].gameObject.transform.position);

            // Update the nearest object
            if (currentDist < nearestDist && IsInteractable(hitColliders[i].gameObject))
            {
                nearestDist = currentDist;
                nearestObject = hitColliders[i].gameObject;
            }

        }// End for

        return nearestObject;


    }

    // Check if game object implements IInteractable
    bool IsInteractable(GameObject gameObject)
    {
        return (gameObject.GetComponent<IInteractable>() != null); 
    }


    void CheckForInteractableObjects()
    {
        if (GetObjectWithinRange() == null)
            canInteract = false;    // Nothing to interact with
        else
            canInteract = true;     // An interactable object is nearby
    }
    

    void InteractWithObject(GameObject gameObject)
    {
        gameObject.GetComponent<IInteractable>().Interact();
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Collectible")
        {
            Debug.Log("stuff collected");
            Destroy(collision.gameObject);
        }
    }

    void HighlightObject(GameObject gameObject)
    {
        gameObject.GetComponent<Renderer>().material.shader = Shader.Find(shaderName);
    }


    // For debugging purposes
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }

    public float GetSpeed()
    {
        return moveSpeedMultiplier;
    }

    

}

