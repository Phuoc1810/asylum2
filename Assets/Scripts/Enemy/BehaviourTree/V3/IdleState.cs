using UnityEngine;
using UnityEngine.AI;

public class IdleState : StateMachineBehaviour
{
    [Header("Idle Settings")]
    [SerializeField] private float idleDuration = 5f;
    [SerializeField] private float detectionRange = 10f;

    private float timer;
    private Transform player;
    private NavMeshAgent agent;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset timer
        timer = 0f;

        // Get components
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Stop agent movement
        if (agent != null)
        {
            agent.SetDestination(agent.transform.position);
            agent.speed = 0f;
            agent.isStopped = true;
        }

        Debug.Log("Entered Idle State");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        // Check for player jumpscare range first
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, animator.transform.position);

            if (distanceToPlayer <= 2.5f)
            {
                animator.SetBool("IsJumpscaring", true);
                Debug.Log("Direct jumpscare from idle");
                return;
            }

            // Check for player detection
            if (distanceToPlayer <= detectionRange)
            {
                animator.SetBool("IsAlert", true);
                return;
            }
        }

        // Transition to patrol after idle duration
        if (timer >= idleDuration)
        {
            animator.SetBool("IsPatrolling", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Re-enable agent when leaving idle
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = 2.5f;
        }

        Debug.Log("Exited Idle State");
    }
}