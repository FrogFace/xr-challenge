﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private int maxHealth = 60;
    private int currentHealth = 60;
    [SerializeField]
    private float rotationSpeed = 10f;
    [SerializeField]
    private float minAttackDistance = 1.5f;
    [SerializeField]
    private float attackCooldown = 3f;
    [SerializeField]
    private bool startUnderground = false;

    [Header("References")]
    [SerializeField]
    private ParticleSystem[] hitEffects;
    [SerializeField]
    private ParticleSystem[] digParticles;

    private Transform player = null;
    private NavMeshAgent agent = null;
    private Animator animator = null;
    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (startUnderground) StartCoroutine(DigOutOfGround());
    }

    // Update is called once per frame
    void Update()
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

    private void EnterDeathState()
    {
        //stop agent and start death animation
        agent.isStopped = true;
        animator.SetTrigger("Die");

        // <-- spawn remains / effects here

        //disable collider and EnemyController
        GetComponentInParent<Collider>().enabled = false;
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

        //<-- update Health Bar UI
    }

    private void RotateToFacePlayer()
    {
        agent.isStopped = true;
        Vector3 playerDirection = player.position - transform.position;
        playerDirection.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirection), rotationSpeed * Time.deltaTime);
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
}