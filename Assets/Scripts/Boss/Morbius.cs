using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morbius : BossFSM , IDamagable
{

    // Particle spawn effects
    public GameObject effectRed;
    public GameObject effectBlue;
    public GameObject burst;


	public float cameraShakeIntensity;
	public float cameraShakeRange;
	public AnimationCurve cameraShakeFalloff;
    
    [Range(1, 5)] public float attackRange;

    private Rigidbody rigidBody;
    private AudioSource source;
    private Animator animator;
    private int health = 15;
    private float maxHealth;

    // Animation flags
    private bool isSpawning;
    private bool isChasing;
    private bool isInRange;
    private bool isDead;

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
		if (GetComponent<HealthEffect>())
        GetComponent<HealthEffect>().ReduceHealth(damage / maxHealth);
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


    protected override void Initialize()
    {
		player = GameObject.FindGameObjectWithTag("Player");
		maxHealth = health;
        currentState = FSMState.Spawn;
        GenerateSpawnEffects();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    protected override void FSMUpdate()
    {
        base.FSMUpdate();
        DebuggingInput();
        if (health <= 0)
        {
            currentState = FSMState.Dead;
        }     

        
    }

    protected override void HandleAnimations()
    {
        animator.SetBool("isChasing", isChasing);
        animator.SetBool("isSwinging", isInRange);
        animator.SetBool("isSpawning", isSpawning);
        animator.SetBool("isDead", isDead);
    }

    protected override void UpdateSpawnState()
    {
        isSpawning = true;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            isSpawning = false;
            currentState = FSMState.Chase;
        }
    }

    protected override void UpdateChaseState()
    {
        isChasing = true;
        transform.LookAt(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            isChasing = false;
            currentState = FSMState.Attack;
        }

    }

    protected override void UpdateAttackState()
    {
        isInRange = true;

        if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
        {
            isInRange = false;
            currentState = FSMState.Chase;
        }

    }

    protected override void UpdateAttack2State()
    {
        Debug.Log("2nd VARIATION");
    }

    protected override void UpdateUltimateState()
    {
        Debug.Log("USING ULT");
    }

    protected override void UpdateDeadState()
    {
        isDead = true;
        isSpawning = false;
        isChasing = false;
    }

    protected void GenerateSpawnEffects()
    {
	    Shake(2, 3);
        Instantiate(effectBlue, transform.position, transform.rotation);
        Instantiate(effectRed, transform.position, transform.rotation);
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
        source.PlayOneShot(Resources.Load("Sounds/BOSS_roar") as AudioClip);
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
		source.PlayOneShot(Resources.Load("Sounds/footsteps1") as AudioClip);
        
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
