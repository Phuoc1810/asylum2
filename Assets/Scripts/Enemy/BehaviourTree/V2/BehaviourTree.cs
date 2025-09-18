using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BehaviourTree : MonoBehaviour
{
    // References - Các tham chiếu
    [Header("References")]
    public Transform playerTransform; // Transform của người chơi
    private NavMeshAgent agent; // Component điều khiển di chuyển trên NavMesh
    private Animator animator; // Component điều khiển animation

    // Behaviour Tree - Cây hành vi
    private Node topNode; // Node gốc của cây hành vi

    // Parameters - Các tham số
    [Header("Detection Parameters")]
    public float detectionRange = 10f; // Phạm vi phát hiện người chơi
    public float attackRange = 2f; // Phạm vi tấn công
    public float chaseStoppingDistance = 0.5f; // Khoảng cách dừng khi chase

    [Header("Patrol Parameters")]
    public float patrolRadius = 10f; // Bán kính tuần tra
    public float minPatrolWaitTime = 1f; // Thời gian chờ tối thiểu tại điểm tuần tra
    public float maxPatrolWaitTime = 3f; // Thời gian chờ tối đa tại điểm tuần tra

    [Header("Attack Parameters")]
    public float attackCooldown = 2f; // Thời gian hồi chiêu tấn công
    private float lastAttackTime; // Thời điểm tấn công cuối cùng

    [Header("Jumpscare Parameters")]
    public float jumpscareRotationTime = 0.3f; // Thời gian xoay camera khi jumpscare

    private void Awake()
    {
        // Lấy các component cần thiết
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
            Debug.LogError("Thiếu NavMeshAgent trên Skeleton!");
        else
        {
            agent.stoppingDistance = chaseStoppingDistance;
            agent.autoBraking = true;
            agent.acceleration = 15f; // Tăng để phanh nhanh
        }

        // Tìm người chơi nếu chưa được gán
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // Khởi tạo cây hành vi
        ConstructBehaviourTree();
    }

    private void ConstructBehaviourTree()
    {
        // Tạo các node lá (leaf nodes)
        IsPlayerInDetectionRange isPlayerInDetectionRange = new IsPlayerInDetectionRange(playerTransform, transform, detectionRange);
        IsPlayerInAttackRange isPlayerInAttackRange = new IsPlayerInAttackRange(playerTransform, transform, attackRange);
        ChasePlayer chasePlayer = new ChasePlayer(playerTransform, transform, agent, animator, "Run", chaseStoppingDistance);
        Patrol patrol = new Patrol(transform, agent, patrolRadius, minPatrolWaitTime, maxPatrolWaitTime, animator);
        AttackPlayer attackPlayer = new AttackPlayer(playerTransform, transform, animator, attackCooldown, ref lastAttackTime);
        Jumpscare jumpscare = new Jumpscare(playerTransform, transform, animator, jumpscareRotationTime);

        // Sequence tấn công: Kiểm tra nếu trong tầm tấn công, sau đó tấn công
        Sequence attackSequence = new Sequence(new List<Node> { isPlayerInAttackRange, attackPlayer });

        // Sequence jumpscare: Kiểm tra nếu trong tầm tấn công, sau đó jumpscare
        Sequence jumpscareSequence = new Sequence(new List<Node> { isPlayerInAttackRange, jumpscare });

        // Selector đuổi theo, tấn công, hoặc jumpscare: Ưu tiên jumpscare > attack > chase
        Selector chaseOrAttackOrJumpscare = new Selector(new List<Node> { jumpscareSequence, attackSequence, chasePlayer });

        // Sequence phát hiện: Nếu phát hiện người chơi, đuổi theo/tấn công/jumpscare
        Sequence detectionSequence = new Sequence(new List<Node> { isPlayerInDetectionRange, chaseOrAttackOrJumpscare });

        // Hành vi cấp cao nhất: Cố gắng phát hiện và hành động, nếu không thì tuần tra
        topNode = new Selector(new List<Node> { detectionSequence, patrol });
    }

    private void Update()
    {
        // Đánh giá cây hành vi mỗi frame
        topNode.Evaluate();
    }

    // Hiển thị trực quan để gỡ lỗi
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}