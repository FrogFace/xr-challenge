using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform player = null;
    private NavMeshAgent agent = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, player.position) > 2f)
        {
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
