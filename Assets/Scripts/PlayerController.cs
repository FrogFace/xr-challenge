﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private float rotationSpeed = 10f;
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private int damageDealt = 20;

    [Header("References")]
    [SerializeField]
    private Collider shieldcollider = null;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip dryFootstepClip = null;
    [SerializeField]
    private AudioClip wetFootstepClip = null;
    [SerializeField]
    private AudioClip swordSwingClip = null;
    [SerializeField]
    private AudioClip hitReactionclip = null;
    [SerializeField]
    private AudioClip itemPickupClip = null;

    private UIManager uiManager = null;
    private GameManager gameManager = null;
    private CharacterController charController = null;
    private Animator animator = null;

    private bool isAttacking = false;
    private bool isBlocking = false;
    private bool isRolling = false;
    private bool allowMovement = true;

    private float currentHealth = 100f;

    // Start is called before the first frame update
    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();
        charController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        uiManager.UpdateHealthBar(Mathf.Lerp(0, maxHealth, currentHealth));
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameManager.isPaused) return;

        HandleBlocking();
        HandleAttacking();
        HandleRolling();

        //disable movement if attacking or blocking
        allowMovement = !(isBlocking || isAttacking || isRolling);

        HandleMovement();

        //update camera above player
        Camera.main.transform.position = transform.position + new Vector3(0, 10, -1);
    }

    private void HandleBlocking()
    {
        isBlocking = Input.GetAxisRaw("Block") == 1;
        animator.SetBool("Blocking", isBlocking);
        shieldcollider.enabled = isBlocking && !isAttacking && !isRolling;
    }

    private void HandleAttacking()
    {
        if (Input.GetButtonDown("Attack"))
        {
            animator.SetTrigger("Attack");
            if (!isAttacking) StartCoroutine(SwordAttack());
        }
    }

    private void HandleRolling()
    {
        if (Input.GetAxisRaw("Roll") == 1 && allowMovement)
        {
            StartCoroutine(RollForward());
        }
    }

    /// <summary>
    /// Moves the characterController using horizontal and vertical Axis.
    /// Updates animation controller with velocity.
    /// will not move player if movement is disabled.
    /// </summary>
    private void HandleMovement()
    {
        //get inputs
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        //create movement vector with movement speed
        Vector3 movementVector = new Vector3(xInput, 0, yInput) * movementSpeed;

        //clamp movespeed to prevent faster diagagonal movement
        movementVector = Vector3.ClampMagnitude(movementVector, movementSpeed);

        //update character controller
        if (allowMovement)
        {
            charController.SimpleMove(movementVector);
        }
        else if (!isRolling)
        {
            charController.SimpleMove(Vector3.zero);
        }

        if (movementVector != Vector3.zero && !isRolling)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), rotationSpeed * Time.deltaTime);
        }

        //update animator speed variable
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), charController.velocity.magnitude, 10 * Time.deltaTime));
    }


    /// <summary>
    /// Rotates player to face velocity if moving otherwise rotates to face mouse position
    /// </summary>
    private void HandleRotation()
    {
        if (charController.velocity.magnitude > 0.2f)
        {
            //get player XZ velocity
            Vector3 dirVec = new Vector3(charController.velocity.x, 0, charController.velocity.z);

            //rotate to face movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVec), rotationSpeed * Time.deltaTime);
        }
        else
        {
            //rotate to face mouse Position
            RotateToFaceWorldPosition(GetMouseWorldPosition());
        }
    }

    /// <summary>
    /// Returns the world position of the mouse cursor
    /// </summary>
    /// <returns>The mouse cursors position in world space, returns vector3.zero if raycast fails to hit</returns>
    private Vector3 GetMouseWorldPosition()
    {
        //get mouse position as world positon
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitData, 1000))
        {
            return hitData.point;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Rotates the player to face the target position over time
    /// </summary>
    /// <param name="targetPosition">The target position to look at</param>
    private void RotateToFaceWorldPosition(Vector3 targetPosition)
    {
        //align height with player
        targetPosition.y = transform.position.y;

        //aim player at world mouse position
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if other object is a pickup item
        if (other.CompareTag("Star") || other.CompareTag("Gold"))
        {
            //pickup star if pick up script found
            Pickup item = other.GetComponent<Pickup>();
            if (item != null)
            {
                int pickUpValue = item.GetPickedUp();

                //only add score if star is valid
                pickUpValue = pickUpValue == -1 ? 0 : pickUpValue;

                //add score to score system
                gameManager.AddScore(pickUpValue);

                if (pickUpValue > 0) AudioSource.PlayClipAtPoint(itemPickupClip, transform.position, 0.5f);
            }
        }
    }

    /// <summary>
    /// Search for colliders in front of player, if player or breakable object attempt to deal damage to them
    /// Should be called as animation event
    /// </summary>
    public void TryDealDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward, 0.8f);

        foreach (Collider col in hitColliders)
        {
            //check if enemy/ breakable object
            //deal dmaamge
            if (col.CompareTag("Enemy"))
            {
                EnemyController enemy = col.GetComponent<EnemyController>();
                if (enemy) enemy.ModifyHealth(damageDealt);
            }
            else if (col.CompareTag("Breakable"))
            {
                BreakableObject breakable = col.GetComponent<BreakableObject>();
                if (breakable) breakable.ModifyHealth(damageDealt);
            }
        }
    }


    /// <summary>
    /// Plays a wet or dry footstep sound depending on current surface 
    /// </summary>
    public void PlayfootstepSound()
    {
        //check if standing in water
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit))
        {
            if (hit.transform.CompareTag("Water"))
            {
                //play wetfootstep
                AudioSource.PlayClipAtPoint(wetFootstepClip, transform.position, 0.3f);
                return;
            }
        }

        //play normal footstep
        AudioSource.PlayClipAtPoint(dryFootstepClip, transform.position, 0.5f);
    }

    /// <summary>
    /// Plays a sword swing sound effect
    /// </summary>
    public void PlaySwordSwingSound()
    {
        //play normal footstep
        AudioSource.PlayClipAtPoint(swordSwingClip, transform.position, 0.5f);
    }

    /// <summary>
    /// Change the current health value of the player 
    /// </summary>
    /// <param name="changeValue">The ammount of health removed</param>
    public void ModifyHealth(int changeValue)
    {
        //reduce current health by value, clamp between 0 and maxHealth
        currentHealth -= changeValue;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        AudioSource.PlayClipAtPoint(hitReactionclip, transform.position, 1);

        //Update HealthBar
        uiManager.UpdateHealthBar(Mathf.InverseLerp(0, maxHealth, currentHealth));

        //Death Check, kill if health is 0
        if (currentHealth <= 0f)
        {
            StartCoroutine(EnterDeathState());
            return;
        }

        //restart attack cooldown if attacking
        if (isAttacking)
        {
            StopCoroutine(SwordAttack());
            isAttacking = false;
        }

        //player stagger animation
        animator.SetTrigger("Stagger");
    }

    #region Coroutines

    private IEnumerator RollForward()
    {
        isRolling = true;

        yield return 0;

        //get inputs
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(xInput, 0, yInput);

        if (movement != Vector3.zero) transform.rotation = Quaternion.LookRotation(movement);

        animator.Play("Roll", 0);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            charController.SimpleMove(transform.forward * 5f);
            yield return 0;
        }


        isRolling = false;
    }

    private IEnumerator SwordAttack()
    {
        //begin attack, disable agent movement
        isAttacking = true;

        //start attack animation
        animator.SetTrigger("Attack");
        yield return 0;

        //wait while transitioning to attack animation
        while (animator.IsInTransition(0)) yield return 0;

        //wait while attack animation playing
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3"))
        {
            //reset attack trigger during first 50% of animation
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f) animator.ResetTrigger("Attack");
            yield return 0;
        }

        isAttacking = false;
    }


    private IEnumerator EnterDeathState()
    {
        //stop agent and start death animation
        animator.SetTrigger("Die");

        //disable collider and EnemyController
        GetComponent<Collider>().enabled = false;
        charController.enabled = false;
        enabled = false;

        yield return new WaitForSeconds(3.5f);

        gameManager.GameOver();
    }

    #endregion
}
