using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private int maxHealth = 60;
    [SerializeField]
    private float rotationSpeed = 10f;
    [SerializeField]
    private float minAttackDistance = 1.5f;
    [SerializeField]
    private float attackCooldown = 3f;
    [SerializeField]
    private bool startUnderground = false;
    [SerializeField]
    private int scoreValue = 100;

    [Header("References")]
    [SerializeField]
    private ParticleSystem[] hitEffects;
    [SerializeField]
    private ParticleSystem[] digParticles;
    [SerializeField]
    private GameObject[] characterModels;
    [SerializeField]
    private GameObject[] weaponModels;
    [SerializeField]
    private GameObject minimapIcon = null;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip swordSwingClip = null;
    [SerializeField]
    private AudioClip shieldHitClip = null;
    [SerializeField]
    private AudioClip diggingClip = null;
    [SerializeField]
    private AudioClip hitReactionclip = null;
    [SerializeField]
    private AudioClip deathClip = null;


    private GameManager gameManager = null;
    private int currentHealth = 60;
    private Transform player = null;
    private NavMeshAgent agent = null;
    private Animator animator = null;
    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        gameManager = FindObjectOfType<GameManager>();

        if (startUnderground) StartCoroutine(DigOutOfGround());
        RandomizeCharacterModel();
    }

    // Update is called once per frame
    private void Update()
    {
        //update animator speed for movement blend tree
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agent.velocity.magnitude, 10 * Time.deltaTime));

        if (!isAttacking)
        {
            if (Vector3.Distance(transform.position, player.position) > minAttackDistance)
            {
                //move agent towards player if out of attack
                MoveToPlayer();
            }
            else
            {
                //turn to face player if in attack range
                RotateToFacePlayer();

                //attack player if cooldown complete
                if (attackCooldownTimer > attackCooldown) StartCoroutine(SwordAttack());
            }
        }

        //update cooldownTimer
        attackCooldownTimer += Time.deltaTime;
    }

    private IEnumerator DigOutOfGround()
    {
        //prevent other behavior runnign while digging animation active
        enabled = false;
        animator.Play("DigOut");
        AudioSource.PlayClipAtPoint(diggingClip, transform.position, 1);

        //Randomise digging animation speed for variation
        animator.SetFloat("DigSpeedModifier", Random.Range(0.8f, 1.2f));

        //play dig particle effects
        foreach (ParticleSystem effect in digParticles) effect.Play();

        //wait while digging animation is playing
        yield return 0;
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("DigOut")) yield return 0;

        //enable other behaviors once digging  animtation finished
        enabled = true;
    }

    private void RandomizeCharacterModel()
    {
        int rndNum = Random.Range(0, characterModels.Length);

        for (int i = 0; i < characterModels.Length; i++)
        {
            if (i == rndNum) characterModels[i].SetActive(true);
            else characterModels[i].SetActive(false);
        }

        rndNum = Random.Range(0, weaponModels.Length);

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (i == rndNum) weaponModels[i].SetActive(true);
            else weaponModels[i].SetActive(false);
        }
    }

    private void EnterDeathState()
    {
        //stop agent and start death animation
        agent.enabled = false;
        animator.SetTrigger("Die");

        AudioSource.PlayClipAtPoint(deathClip, transform.position, 0.6f);

        //disable collider and EnemyController
        GetComponent<Collider>().enabled = false;
        StartCoroutine(DespawnDeadEnemy());


        gameManager.AddScore(scoreValue);
        enabled = false;
    }

    private void MoveToPlayer()
    {
        agent.SetDestination(player.position);
        agent.isStopped = false;
    }

    /// <summary>
    /// Change the current health value of this enemy 
    /// </summary>
    /// <param name="changeValue">The ammount of health removed</param>
    public void ModifyHealth(int changeValue)
    {
        //reduce current health by value, clamp between 0 and maxHealth
        currentHealth -= changeValue;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //play hit particle effects
        foreach (ParticleSystem effect in hitEffects) effect.Play();

        AudioSource.PlayClipAtPoint(hitReactionclip, transform.position, 1);

        //Death Check, kill if health is 0
        if (currentHealth <= 0f)
        {
            EnterDeathState();
            return;
        }

        //restart attack cooldown if attacking
        if (isAttacking)
        {
            StopCoroutine(SwordAttack());
            isAttacking = false;
            attackCooldownTimer = 0f;
        }

        //player stagger animation
        animator.SetTrigger("Stagger");

        
    }


    public void TryAttackPlayer()
    {
        bool hitShield = false;
        bool hitPlayer = false;

        Collider[] colliderHitArray = Physics.OverlapSphere(transform.position + transform.forward + transform.up, 0.7f);
        foreach (Collider col in colliderHitArray)
        {
            if (col.CompareTag("Shield")) hitShield = true;
            if (col.CompareTag("Player")) hitPlayer = true;

        }

        if (hitShield)
        {
            StopCoroutine(SwordAttack());
            isAttacking = false;
            attackCooldownTimer = 0f;

            //player stagger animation
            animator.SetTrigger("Stagger");

            AudioSource.PlayClipAtPoint(shieldHitClip, transform.position, 1);
        }
        else if (hitPlayer)
        {
            player.GetComponent<PlayerController>().ModifyHealth(10);
        }
    }

    private void RotateToFacePlayer()
    {
        if (agent.isOnNavMesh) agent.isStopped = true;
        Vector3 playerDirection = player.position - transform.position;
        playerDirection.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirection), rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Plays a sword swing sound effect
    /// </summary>
    public void PlaySwordSwingSound()
    {
        //play normal footstep
        AudioSource.PlayClipAtPoint(swordSwingClip, transform.position, 0.5f);
    }

    private IEnumerator SwordAttack()
    {
        //begin attack, disable agent movement
        isAttacking = true;
        agent.isStopped = true;

        //start attack animation
        animator.SetTrigger("Attack");
        yield return 0;

        //wait while attack animation playing
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //rotate enemy to face player for first 20% of animation
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f) RotateToFacePlayer();
            yield return 0;
        }

        //restart attackCooldown
        attackCooldownTimer = 0;
        isAttacking = false;
    }

    private IEnumerator DespawnDeadEnemy()
    {
        minimapIcon.SetActive(false);

        //wait for 5 seconds
        yield return new WaitForSeconds(2.5f);

        //move enemy body underground slowly for 5 seconds
        float t = 0f;
        while (t <= 5f)
        {
            t += Time.deltaTime;

            //move enemy body downwards
            transform.position -= Vector3.up * 0.2f * Time.deltaTime;
            yield return 0;
        }

        //disable the gameobject
        this.gameObject.SetActive(false);
    }
}
