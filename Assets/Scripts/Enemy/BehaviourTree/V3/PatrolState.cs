using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrolState : StateMachineBehaviour
{
    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer; // Gán layer "Wall" trong Inspector

    float timer;
    List<Transform> patrolList = new List<Transform>();
    NavMeshAgent agent;
    Transform player;
    float chaseRange = 10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 2.5f;
        timer = 0;
        GameObject go = GameObject.FindGameObjectWithTag("Patrol point");
        foreach (Transform t in go.transform)
            patrolList.Add(t);

        agent.SetDestination(patrolList[Random.Range(0, patrolList.Count)].position);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            agent.SetDestination(patrolList[Random.Range(0, patrolList.Count)].position);
        timer += Time.deltaTime;

        // Check for player jumpscare range first
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance <= 2.5f)
        {
            animator.SetBool("IsJumpscaring", true);
            Debug.Log("Direct jumpscare from patrol");
            return;
        }

        if (timer > 10)
            animator.SetBool("IsPatrolling", false);

        // ✅ THÊM CHECK LINE OF SIGHT
        if (distance < chaseRange && CanSeePlayer(animator))
        {
            animator.SetBool("IsAlert", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
    }

    // ✅ METHOD MỚI: Check xem có tường chắn không
    bool CanSeePlayer(Animator animator)
    {
        if (player == null) return false;

        float distance = Vector3.Distance(player.position, animator.transform.position);
        Vector3 direction = (player.position - animator.transform.position).normalized;

        // Raycast từ vị trí mắt Agent (thêm Vector3.up để tránh hit ground)
        Vector3 startPos = animator.transform.position + Vector3.up * 1.5f;

        // Raycast để check tường
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, distance, wallLayer))
        {
            // Nếu hit tường trước khi tới Player = có tường chắn
            Debug.DrawRay(startPos, direction * hit.distance, Color.red, 0.1f);
            return false;
        }

        // Không có tường chắn
        Debug.DrawRay(startPos, direction * distance, Color.green, 0.1f);
        return true;
    }
}