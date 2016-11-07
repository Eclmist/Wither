using UnityEngine;
using System.Collections;

class PlayerController : MonoBehaviour
{
    [Range(0,30)]
    [SerializeField]
    private float moveSpeedMultiplier;

    private Rigidbody rigidBody;
    private Animator animator;
    private bool isMoving;
    private int health;

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

    public int Health
    {
        get { return this.health; }
        set { this.health = value; }
    }


	// Use this for initialization
	void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isMoving = false;

	}
	
	// Update is called once per frame
	void Update ()
    {
        
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        HandleInput();
        HandleAnimations();

        hitColliders = Physics.OverlapSphere(this.transform.position,sphereRadius,1);
        CheckForInteractableObjects();

    }

    void FixedUpdate()
    {
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
            rigidBody.velocity = new Vector3(horizontal * moveSpeedMultiplier, rigidBody.velocity.y,vertical * moveSpeedMultiplier);
        }

        if (horizontal == 0 && vertical == 0)
            isMoving = false;
 
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

