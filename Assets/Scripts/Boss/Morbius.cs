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

	public float cameraShakeIntensity;
	public float cameraShakeRange;
	public AnimationCurve cameraShakeFalloff;
    public float moveSpeed;
    
    [Range(1, 5)] public float attackRange;

    private Rigidbody rigidBody;
    private AudioSource source;
    private Animator animator;
    private int health = 15;
    private float maxHealth;
    private int damageMultiplier = 1;
    private int stance = 1;

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
            stance = 2;
            attackRange *= 3;
            health = 9999;
            damageMultiplier = 0;
            isChasing = false;
            isInRange = false;
            currentState = FSMState.Spawn;
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

    protected override void UpdateSpawnState()
    {
        isSpawning = true;

        
  
    }

    protected override void UpdateChaseState()
    {
        isChasing = true;
        Quaternion rot = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2.0f * Time.deltaTime);

        rigidBody.velocity = transform.forward * moveSpeed;



        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            isChasing = false;
                currentState = FSMState.Attack;

            
        }


    }

    protected override void UpdateAttackState()
    {

        // if out of range
        if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
            {
               isInRange = false;
                currentState = FSMState.Chase;
            }
            else
            {
                if (Vector3.Angle(transform.forward, player.transform.position - transform.position) < 60)
                {
                    if (stance == 1)
                    {
                        isInRange = true;
                        //rigidBody.constraints = RigidbodyConstraints.FreezeAll;
                    }
                    else
                        isPounding = true;
                }
                   
                 
            }




           
    }

    protected override void UpdateAttack2State()
    {
    }

    protected override void UpdateUltimateState()
    {
        Debug.Log("USING ULT");

        isSpawning = true;
        isChasing = false;
        
     
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
        Instantiate(fissure,transform.position,transform.rotation);
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
