using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morbius : BossFSM , IDamagable
{

    // Particle spawn effects
    public GameObject effectRed;
    public GameObject effectBlue;
    public GameObject burst;
    public GameObject bossGUI;
    public GameObject castPoint;

	public float cameraShakeIntensity;
	public float cameraShakeRange;
	public AnimationCurve cameraShakeFalloff;
    public float moveSpeed;
    [Range(1, 5)] public float attackRange;
    [Range(1,360)]public float attackAngle;
    public float angleCorrectionTurnRate;
    public float rotationSpeed;
    // Avoidance
    public float minimumDistToAvoid = 5;
    public LayerMask avoidanceIgnoreMask;
    public float force = 10;
	private Avoidance avoidance;


    private Rigidbody rigidBody;
    private AudioSource source;
    private Animator animator;
    private int health = 300;
    private float maxHealth;
    private int damageMultiplier = 1;
    public AttackStance currentAttackStance;
	public enum AttackStance { NORMAL,BROKEN}

    // Animation flags
    private bool isSpawning;
    private bool isChasing;
    private bool IsSwinging;
    private bool isDead;
    private bool isPounding;

    //Resources
    private AudioClip roar_clip;
    private AudioClip footstep_clip;
    private GameObject fissure;
	public AudioClip fissure_clip;


	//Actually hitting player
	private Player playerHealth;
	[SerializeField] private int swingDamage = 35;

    public int Health
    {
        get { return this.health; }
    }

    void DebuggingInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.R))
            TakeDamage(50);

	    if (Input.GetKeyDown(KeyCode.T))
		    ForceKill();
    }

	public void TakeDamage(int damage)
    {
        if (GetComponent<HealthEffect>() != null)
            GetComponent<HealthEffect>().ReduceHealth(damage / maxHealth);
        else
            Debug.Log("boss does not have HealthEffect Component");

        health -= damage * damageMultiplier;
    }


	public void ForceKill()
	{
		currentState = FSMState.Dead;
	}


	protected override void Initialize()
    {
        roar_clip =  Resources.Load("Sounds/BOSS_roar") as AudioClip;
        footstep_clip = Resources.Load("Sounds/footsteps1") as AudioClip;
        fissure = Resources.Load("Skills/FireFissure") as GameObject;
		player = GameObject.FindGameObjectWithTag("Player");
	    playerHealth = player.GetComponent<Player>();
		maxHealth = health;
        currentState = FSMState.Chase;
        currentAttackStance = AttackStance.NORMAL;
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
	    avoidance = GetComponent<Avoidance>();

    }

    protected override void FSMUpdate()
    {
        base.FSMUpdate();
        DebuggingInput();


        bossGUI.SetActive(Vector3.Distance(player.transform.position,transform.position) <= cameraShakeRange);
		if (health <= 0)
        {
            currentAttackStance = AttackStance.BROKEN;
            attackRange *= 4;
	        attackAngle  = 10;
            health = 600;
            isChasing = false;
            IsSwinging = false;
            currentState = FSMState.Overpowered;
        }     

        
    }

    protected override void HandleAnimations()
    {
        animator.SetBool("isChasing", isChasing);
        animator.SetBool("isSwinging", IsSwinging);
        animator.SetBool("isSpawning", isSpawning);
        animator.SetBool("isDead", isDead);
        animator.SetBool("isPounding", isPounding);
    }

    protected override void UpdateOverpoweredState()
    {
        isSpawning = true;
    }

    protected override void UpdateChaseState()
    {
        isChasing = true;


        //Quaternion rot = Quaternion.LookRotation(player.transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2.0f * Time.deltaTime);

        if(avoidance != null)
			avoidance.LookAtPlayer();


        rigidBody.velocity = transform.forward * moveSpeed;

		if (PlayerWithinAngle() && PlayerWithinRange())
        {
            // If the player is within range and angle is correct
            isChasing = false;
            currentState = FSMState.Attack;
        }
        else if(PlayerWithinRange())
        {
            // If player is within range but not in the right angle
            currentState = FSMState.Turn;
        }
    }

    

    protected override void UpdateAttackState()
    {
        // If the player is out of range OR not in the right angle to hit
        if (PlayerWithinAngle() && PlayerWithinRange())
		{
			Attack();
        }
		else if (!PlayerWithinRange() && (!isPounding && !IsSwinging))
		{
			currentState = FSMState.Chase;
		}
		else if (!isPounding && !IsSwinging)
		{
			currentState = FSMState.Turn;
		}
	}

    protected override void UpdateTurnState()
    {
	    Vector3 vectorToPlayer = player.transform.position - transform.position;
	    vectorToPlayer.y = 0;


		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vectorToPlayer)
            , angleCorrectionTurnRate * Time.deltaTime);

        if(PlayerWithinAngle() && PlayerWithinRange())
        {
            currentState = FSMState.Attack;
        }

	    if (!PlayerWithinRange())
	    {
		    currentState = FSMState.Chase;
	    }
    }

    protected override void UpdateDeadState()
    {
        isDead = true;
        isSpawning = false;
        isChasing = false;
    }

    protected void GenerateSpawnEffects()
    {
	    Shake(2, 4);
        Instantiate(effectBlue, transform.position, transform.rotation);
        Instantiate(effectRed, transform.position, transform.rotation);
    }

    bool IsInMotion(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    private void Attack()
    {
        //Check type of attack based on stance
        switch(currentAttackStance)
        {
            case AttackStance.NORMAL:
                IsSwinging = true;
                break;
            case AttackStance.BROKEN:
                isPounding = true;
                break;
        }
    }

	private bool PlayerWithinRange()
	{
		return Vector3.Distance(transform.position, player.transform.position) <= attackRange;
	}

	private bool PlayerWithinAngle()
	{
		Vector3 forward = transform.forward;
		forward.y = 0;

		Vector3 playerPos = player.transform.position - transform.position;
		playerPos.y = 0;


		return Vector3.Angle(forward, playerPos) <= attackAngle;
	}

	//-------------------------------------------- Animation events -------------------------------------------------------------------------------//

	public void TurnOnBurst()
    {
        burst.SetActive(true);
    }

    public void TurnOffBurst()
    {
        burst.SetActive(false);
    }

    public void HeavySwing()
    {
	    if (PlayerWithinRange() && PlayerWithinAngle())
	    {
			Shake(0.5f, 1);
			playerHealth.ReduceHealth(swingDamage);
		}

	}

    public void Roar()
    {
        GenerateSpawnEffects();
        if(roar_clip != null)
        source.PlayOneShot(roar_clip);

    }

    public void Falling()
    {
		Shake(0.5f, 1);
	}

	public void Fall()
    {
		Shake(1, 2);
	}

	public void TakeStep()
    {
		Shake(0.2F, 1);
        if (footstep_clip != null)
            source.PlayOneShot(footstep_clip);
        
    }

    public void CheckPlayerPosition()
    {
        IsSwinging = false;
		isPounding = false;

		//rigidBody.constraints -= RigidbodyConstraints.FreezePosition;

	}

	public void TransitionToChase()
    {
		isChasing = true;
		isSpawning = false;
        currentState = FSMState.Chase;
    }

    public void Fissure()
    {
	    AudioManager.Instance.PlaySound(fissure_clip, gameObject);

        isPounding = false;

        if(fissure != null)
        Instantiate(fissure,castPoint.transform.position,transform.rotation);

		Shake(0.5f,4);
    }

	public void Shake(float duration, float amount)
	{
		float distanceFromPlayer = Vector3.Distance(player.transform.position,
			transform.position);

		float ratio = distanceFromPlayer/cameraShakeRange;

		float unmodifiedIntensity = cameraShakeFalloff.Evaluate(ratio);

		float finalIntensity = unmodifiedIntensity * cameraShakeIntensity *amount;
		Effects.instance.ShakeCameraRelative(duration, finalIntensity);

	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color (1,1,0,0.3f);
		Gizmos.DrawSphere(transform.position, cameraShakeRange);
	}


}
