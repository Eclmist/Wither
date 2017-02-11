using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamagable
{

    public static Player Instance;

    public int maxHealth = 100;
    public int maxMana = 100;
    public int maxStamina = 100;

    public int healthRestoreRate = 1;
    public int manaRestoreRate = 1;
    public int staminaRestoreRate = 1;

    public AudioSource audioSource;

    [Header("Audio Clips")]
    // Player resources
    public AudioClip ac_footstep1;
    public AudioClip ac_footstep2;
    public AudioClip ac_hurt;
    public AudioClip ac_test;
    public GameObject elem;

    private int currentHealth;
    private int currentMana;
    private int currentStamina;

    private float timeSinceLastDamageTaken;
    private float timeSinceLastStaminaUse;
    private float timeSinceLastManaUse;


    void Awake()
    {
        Instance = this;
  
    }

	// Use this for initialization
	void Start () {

        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;

        timeSinceLastDamageTaken = 0;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.X))
            AudioManager.Instance.PlaySoundAt(ac_test,elem.transform.position,elem);

	    if (currentHealth > 0)
	    {
		    timeSinceLastDamageTaken += Time.deltaTime;
		    timeSinceLastManaUse += Time.deltaTime;
		    timeSinceLastStaminaUse += Time.deltaTime;

		    if (currentMana < 0)
			    currentMana = 0;

		    if (currentStamina < 0)
			    currentStamina = 0;


		    HurtOverlay.Instance.SetOpacity((maxHealth - currentHealth)/(float)maxHealth);

	    }
	    else
	    {
			GetComponent<PlayerController>().isDead = true;
	    }
    }

    void FixedUpdate()
    {
        if (timeSinceLastStaminaUse > 5 && currentStamina < maxStamina)
        {
            currentStamina += staminaRestoreRate;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        if (timeSinceLastManaUse > 5 && currentMana < maxMana)
        {
            currentMana += manaRestoreRate;
            if (currentMana > maxMana)
                currentMana = maxMana;

        }

        if (timeSinceLastDamageTaken > 10 && currentHealth < maxHealth)
        {
            currentHealth += healthRestoreRate;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }

    public void ReduceHealth(int amount)
    {
        timeSinceLastDamageTaken = 0;
        currentHealth -= amount;

        AudioManager.Instance.PlaySound(ac_hurt,gameObject);
    }

    public void ReduceMana(int amount)
    {
        timeSinceLastManaUse = 0;
        currentMana -= amount;
    }

    public void ReduceStamina(int amount)
    {
        timeSinceLastStaminaUse = 0;
        currentStamina -= amount;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetCurrentMana()
    {
        return currentMana;
    }

    public int GetCurrentStamina()
    {
        return currentStamina;
    }

	public void TakeDamage(int damage)
	{
		ReduceHealth(damage);
	}
 
    

    //---------- Animation Events ----------------------------------------------------------------------------//

    public void Step1()
    {
        AudioManager.Instance.PlaySound(ac_footstep1,gameObject);

    }

    public void Step2()
    {
        AudioManager.Instance.PlaySound(ac_footstep2, gameObject);
    }
}
