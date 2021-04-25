using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField]
    private Collider shieldcollider = null;


    ScoreSystem scoreSystem = null;
    CharacterController charController = null;
    Animator animator = null;

    private bool isAttacking = false;
    private bool isBlocking = false;
    private bool isRolling = false;
    private bool allowMovement = true;


    // Start is called before the first frame update
    private void Start()
    {
        scoreSystem = FindObjectOfType<ScoreSystem>();
        charController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleBlocking();
        HandleAttacking();
        HandleRolling();

        //disable movement if attacking or blocking
        allowMovement = !(isBlocking || isAttacking || isRolling);

        HandleMovement();
    }

    private void HandleBlocking()
    {
        isBlocking = Input.GetAxisRaw("Block") == 1;
        animator.SetBool("Blocking", isBlocking);
        shieldcollider.enabled = isBlocking && !isAttacking && !isRolling;
    }

    private void HandleAttacking()
    {
        if (Input.GetAxisRaw("Attack") == 1)
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
        isAttacking = true;

        yield return new WaitForSeconds(0.55f);

        TryDealDamage();

        animator.ResetTrigger("Attack");

        //Resets axis to prevent juttering animation
        //Input.ResetInputAxes();
        isAttacking = false;
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
        //check if other object is a star
        if (other.CompareTag("Star"))
        {
            //pickup star if pick up script found
            Pickup star = other.GetComponent<Pickup>();
            if (star != null)
            {
                int pickUpValue = star.GetPickedUp();

                //only add score if star is valid
                pickUpValue = pickUpValue == -1 ? 0 : pickUpValue;

                //add score to score system
                scoreSystem.AddScore(pickUpValue);
            }
        }
    }

    private void TryDealDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward, 0.8f);

        foreach (Collider col in hitColliders)
        {
            //check if enemy/ breakable object
            //deal dmaamge
            if (col.CompareTag("Enemy"))
            {
                col.GetComponent<EnemyController>().ModifyHealth(20);
            }
        }
    }
}
