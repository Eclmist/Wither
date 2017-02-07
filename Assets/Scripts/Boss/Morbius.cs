using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morbius : BossFSM , IDamagable
{

    // Particle spawn effects
    public GameObject effectRed;
    public GameObject effectBlue;
    public GameObject burst;
    
    [Range(1, 5)] public float attackRange;

    private Rigidbody rigidBody;
    private AudioSource source;
    private Animator animator;
    private float health = 15;
    private float maxHealth;

    // Animation flags
    private bool isSpawning;
    private bool isChasing;
    private bool isInRange;
    private bool isDead;

    public float Health
    {
        get { return this.health; }
    }

    void DebuggingInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TakeDamage(0.1f);
        if (Input.GetKeyDown(KeyCode.R))
            TakeDamage(5f);
    }

    public void TakeDamage(float damage)
    {
		if (GetComponent<HealthEffect>())
        GetComponent<HealthEffect>().ReduceHealth(damage / maxHealth);
        health -= damage;
    }

    protected override void Initialize()
    {
        maxHealth = health;
        currentState = FSMState.Spawn;
        GenerateSpawnEffects();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
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
        Effects.instance.ShakeCameraRelative(2, 0.3f);
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
        Effects.instance.ShakeCameraRelative(0.5f, 0.1f);
    }

    public void Roar()
    {
        source.PlayOneShot(Resources.Load("Sounds/BOSS_roar") as AudioClip);
    }

    public void Falling()
    {
        Effects.instance.ShakeCameraRelative(0.3f, 0.05f);
    }

    public void Fall()
    {
        Effects.instance.ShakeCameraRelative(1f, 0.2f);
    }

    public void TakeStep()
    {
        Effects.instance.ShakeCameraRelative(0.2f, 0.1f);
        source.PlayOneShot(Resources.Load("Sounds/footsteps1") as AudioClip);
        
    }
    
}
