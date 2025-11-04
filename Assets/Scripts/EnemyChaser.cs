using UnityEngine;
using UnityEngine.AI;

public class EnemyChaser : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float detectionRange = 15f;
    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float repathTime = 0.25f;

    NavMeshAgent agent;
    float t;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    void Update()
    {
        t += Time.deltaTime;
        if (t < repathTime) return;
        t = 0f;

        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= detectionRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir.y = 0f;
            agent.SetDestination(transform.position + randomDir);
        }
    }

    void OnCollisionEnter(Collision col)
{
    if (!col.collider.CompareTag("Player")) return;
    if (GameDirector.I != null) GameDirector.I.PlayerCaught();
}

void OnTriggerEnter(Collider other)
{
    if (!other.CompareTag("Player")) return;
    if (GameDirector.I != null) GameDirector.I.PlayerCaught();
}



}
