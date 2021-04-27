using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Material closedMaterial = null;
    [SerializeField]
    private Material openMaterial = null;
    [SerializeField]
    private Pickup[] pickupArray;

    private MeshRenderer meshRenderer = null;
    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        //get the mesh render and set to closed material
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = closedMaterial;

        //subscribe to all of the star pickup events
        foreach (Pickup star in pickupArray) star.OnPickUp += UnlockCheck;
    }

    /// <summary>
    /// Check if all stars have been collected.
    /// Unlocks door if all have been collected.
    /// </summary>
    /// <param name="obj"></param>
    private void UnlockCheck(Pickup obj)
    {
        //loop through all stars and count each collected
        int totalCollected = 0;
        for (int i = 0; i < pickupArray.Length; i++)
        {
            if (pickupArray[i].IsCollected) totalCollected++;
        }

        //if all stars are collected open door
        if (totalCollected == pickupArray.Length) isOpen = true;

        //update material to open
        meshRenderer.material = isOpen ? openMaterial : closedMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        //complete level if exit is open and player is detected
        if (isOpen && other.CompareTag("Player"))
        {
            //Exit/Complete Level
            Time.timeScale = 0;
        }
    }
}
