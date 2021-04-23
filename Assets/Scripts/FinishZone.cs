﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Material closedMaterial = null;
    [SerializeField]
    private Material openMaterial = null;

    private MeshRenderer meshRenderer = null;
    private List<Pickup> pickupList = new List<Pickup>();
    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        //get the mesh render and set to closed material
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = closedMaterial;

        //find all of the stars in the scene adn add themt o the list
        pickupList.Clear();
        pickupList.AddRange(FindObjectsOfType<Pickup>());

        //subscribe to all of the star pickup events
        foreach (Pickup star in pickupList) star.OnPickUp += UnlockCheck;
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
        for (int i = 0; i < pickupList.Count; i++)
        {
            if (pickupList[i].IsCollected) totalCollected++;
        }

        //if all stars are collected open door
        if (totalCollected == pickupList.Count) isOpen = true;

        //update material to open
        meshRenderer.material = isOpen ? openMaterial : closedMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        //complete level if exit is open and player is detected
        if (isOpen && other.CompareTag("Player"))
        {
            //Exit/Complete Level
        }
    }
}