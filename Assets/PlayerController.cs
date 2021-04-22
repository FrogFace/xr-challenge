using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private float movementSpeed = 1f;


    //[Header("References")]

    [SerializeField]
    private int currentScore = 0;
    CharacterController charController = null;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //get inputs
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        //create movement vector with movement speed
        Vector3 movementVector = new Vector3(xInput, 0, yInput) * movementSpeed;

        //update character controller
        charController.SimpleMove(movementVector);
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
                currentScore += pickUpValue == -1 ? 0 : pickUpValue;
            }
        }
    }
}
