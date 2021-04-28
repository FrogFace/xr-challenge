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
    private AudioClip woodBreakClip = null;

    private int currentHealth = 40;

    /// <summary>
    /// Change the current health value of this item 
    /// </summary>
    /// <param name="changeValue">The ammount of health removed</param>
    public void ModifyHealth(int changeValue)
    {
        //reduce current health by value, clamp between 0 and maxHealth
        currentHealth -= changeValue;

        //play audio clip
        AudioSource.PlayClipAtPoint(woodBreakClip, transform.position, 0.6f);

        if(currentHealth > 0)
        {
            //enable damaged model if not destroyed
            normalModel.SetActive(false);
            damagedModel.SetActive(true);
        }
        else
        {
            //enabled destroyed model if destoryed
            damagedModel.SetActive(false);
            destroyedModel.SetActive(true);

            //prevent further hits
            GetComponent<Collider>().enabled = false;
            enabled = false;
        }
    }
}
