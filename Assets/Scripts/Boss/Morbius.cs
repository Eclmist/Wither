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

    private Rigidbody rigidBody;
    private AudioSource source;
    private Animator animator;
    private int health = 15;
    private float maxHealth;
    private int damageMultiplier = 1;
    private AttackStance currentAttackStance;
    private enum AttackStance { NORMAL,BROKEN}

    // Animation flags
    private bool isSpawning;
    private bool isChasing;
    private bool isInRange;
    private bool isDead;
    private bool isPounding;

    //Resources
    private AudioClip roar_clip;
    private AudioClip footstep_clip;
    private GameObject fissure;

    public int Health
    {
        get { return this.health; }
    }

    void DebuggingInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.R))
            TakeDamage(5);
    }

    public void TakeDamage(int damage)
    {
        if (GetComponent<HealthEffect>() != null)
            GetComponent<HealthEffect>().ReduceHealth(damage / maxHealth);
        else
            Debug.Log("boss does not have HealthEffect Component");

        health -= damage * damageMultiplier;
    }



    protected override void Initialize()
    {
        roar_clip =  Resources.Load("Sounds/BOSS_roar") as AudioClip;
        footstep_clip = Resources.Load("Sounds/footsteps1") as AudioClip;
        fissure = Resources.Load("Skills/FireFissure") as GameObject;

        player = GameObject.FindGameObjectWithTag("Player");
		maxHealth = health;
        currentState = FSMState.Chase;
        currentAttackStance = AttackStance.NORMAL;
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
    }

    protected override void FSMUpdate()
    {
        base.FSMUpdate();
        DebuggingInput();

        bossGUI.SetActive(Vector3.Distance(player.transform.position,transform.position) <= cameraShakeRange);

        if (health <= 0)
        {
            Debug.Log("dieded");
            currentAttackStance = AttackStance.BROKEN;
            attackRange *= 2;
            health = 9999;
            damageMultiplier = 0;
            isChasing = false;
            isInRange = false;
            currentState = FSMState.Overpowered;
        }     

        
    }

    protected override void HandleAnimations()
    {
        animator.SetBool("isChasing", isChasing);
        animator.SetBool("isSwinging", isInRange);
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

        if(GetComponent<Avoidance>() != null)
            GetComponent<Avoidance>().LookAtPlayer();


        rigidBody.velocity = transform.forward * moveSpeed;
        
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange &&
            Vector3.Angle(transform.forward, player.transform.position - transform.position)
        <= attackAngle)
        {
            // If the player is within range and angle is correct
            isChasing = false;
            currentState = FSMState.Attack;
        }
        else if(Vector3.Distance(transform.position, player.transform.position) <= attackRange &&
             Vector3.Angle(transform.forward, player.transform.position - transform.position) > attackAngle)
        {
            // If player is within range but not in the right angle
            currentState = FSMState.Turn;


        }

       


    }

    

    protected override void UpdateAttackState()
    {
        // If the player is out of range OR not in the right angle to hit
        if (Vector3.Distance(transform.position, player.transform.position) > attackRange ||
        Vector3.Angle(transform.forward, player.transform.position - transform.position)
         > attackAngle)
        {

            currentState = FSMState.Chase;
           
        }
        else
        {
            Attack();
        }
    }

    


    protected override void UpdateTurnState()
    {
       
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position)
            , angleCorrectionTurnRate * Time.deltaTime);

        if(Vector3.Angle(transform.forward, player.transform.position - transform.position) <= attackAngle)
        {
            currentState = FSMState.Attack;
        }

    }

    protected override void UpdatePatrolState()
    {
        
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
                isInRange = true;
                break;
            case AttackStance.BROKEN:
                isPounding = true;
                break;
        }
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
		Shake(0.5f, 1);
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
        isInRange = false;
        //rigidBody.constraints -= RigidbodyConstraints.FreezePosition;

    }

    public void TransitionToChase()
    {
        isSpawning = false;
        currentState = FSMState.Chase;
    }

    public void Fissure()
    {
        isPounding = false;

        if(fissure != null)
        Instantiate(fissure,castPoint.transform.position,transform.rotation);
        Debug.Log("ended");
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
