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
    private UIManager uiManager = null;
    [SerializeField]
    private GameManager gameManager = null;

    private MeshRenderer meshRenderer = null;
    private bool isOpen = false;

    // Start is called before the first frame update
    private void Start()
    {
        //get the mesh render and set to closed material
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = closedMaterial;
    }

    public void UnlockExit()
    {
        isOpen = true;

        //update material to open
        meshRenderer.material = isOpen ? openMaterial : closedMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        //complete level if exit is open and player is detected
        if (isOpen && other.CompareTag("Player"))
        {
            gameManager.CompleteLevel();
        }
        else if (other.CompareTag("Player"))
        {
            uiManager.SetExitHint(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.SetExitHint(false);
        }

    }
}
