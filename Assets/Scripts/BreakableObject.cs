using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject normalModel = null;
    [SerializeField]
    private GameObject damagedModel = null;
    [SerializeField]
    private GameObject destroyedModel = null;
    [SerializeField]
    private int currentHealth = 40;

    /// <summary>
    /// Change the current health value of this item 
    /// </summary>
    /// <param name="changeValue">The ammount of health removed</param>
    public void ModifyHealth(int changeValue)
    {
        //reduce current health by value, clamp between 0 and maxHealth
        currentHealth -= changeValue;

        if(currentHealth > 0)
        {
            normalModel.SetActive(false);
            damagedModel.SetActive(true);
        }
        else
        {
            damagedModel.SetActive(false);
            destroyedModel.SetActive(true);
            GetComponent<Collider>().enabled = false;
            enabled = false;
        }
    }
}
