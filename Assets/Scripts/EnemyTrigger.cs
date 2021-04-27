using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject enemyWave;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyWave.SetActive(true);
        }
    }
}
