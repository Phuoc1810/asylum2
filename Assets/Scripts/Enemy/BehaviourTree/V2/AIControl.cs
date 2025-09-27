using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float chaseRange = 15f;
    public float jumpscareRange = 5f; // Đồng bộ với stoppingDistance của ChasePlayer
    public float fieldOfViewAngle = 120f;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;

    // Private variables
    private BehaviorTree behaviorTree;
    private Transform player;
    private Transform playerCamera;
    private bool isPlayerInRange = false;
    private bool isPlayerInJumpscareRange = false;
    private bool isHiding = false;

    // Components
    private NavMeshAgent navAgent;
    private Animator animator;

    void Start()
    {
        // Tìm player và playerCamera
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerCamera = Camera.main?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Ensure GameObject has 'Player' tag.");
            return;
        }
        else
        {
            Debug.Log($"Player found at position: {player.position}");
        }

        if (playerCamera == null)
        {
            Debug.LogError("Player camera not found! Ensure main camera is tagged 'MainCamera'.");
            return;
        }
        else
        {
            Debug.Log($"Player camera found at position: {playerCamera.position}");
        }

        // Lấy components
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent not found on enemy!");
            return;
        }
        if (!navAgent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on NavMesh!");
            return;
        }

        if (animator == null)
        {
            Debug.LogWarning("Animator not found, animations won't play.");
        }
        else
        {
            // Kiểm tra animation states tại runtime
            bool hasJumpscare = HasAnimationState("Jumpscare");
            bool hasRun = HasAnimationState("Run");
            bool hasWalk = HasAnimationState("Walk");
            bool hasIdle = HasAnimationState("Idle");
            Debug.Log($"Animator states: Jumpscare={hasJumpscare}, Run={hasRun}, Walk={hasWalk}, Idle={hasIdle}");
        }

        // Đặt tốc độ mặc định
        navAgent.speed = patrolSpeed;

        // Khởi tạo Behavior Tree
        InitializeBehaviorTree();
    }

    bool HasAnimationState(string stateName)
    {
        if (animator == null) return false;
        int stateID = Animator.StringToHash(stateName);
        return animator.HasState(0, stateID);
    }

    void Update()
    {
        // Cập nhật thông tin về player
        UpdatePlayerDetection();

        // Xử lý Behavior Tree
        if (behaviorTree != null)
        {
            NodeStatus status = behaviorTree.Process();
            Debug.Log($"BehaviorTree status: {status}, Jumpscare={isPlayerInJumpscareRange}, Chase={isPlayerInRange}, Hiding={isHiding}");
        }
        else
        {
            Debug.LogWarning("BehaviorTree is null!");
        }
    }

    void InitializeBehaviorTree()
    {
        if (player == null || playerCamera == null || navAgent == null)
        {
            Debug.LogError("Cannot initialize BehaviorTree due to missing components!");
            return;
        }

        // Tạo root node
        behaviorTree = new BehaviorTree("Enemy AI");

        // Tạo root selector
        var rootSelector = new Selector("Root Selector");

        // Branch 1: Jumpscare
        var jumpscareSequence = new Sequence("Jumpscare Sequence", 2); // Priority cao
        var closeToPlayerNode = new CloseToPlayer(transform, player, jumpscareRange, "CloseToPlayer", 2);
        Jumpscare jumpscareNode = new Jumpscare(player, transform, animator, playerCamera, "Jumpscare", 0.3f, "Jumpscare", 2);
        jumpscareSequence.AddChild(closeToPlayerNode);
        jumpscareSequence.AddChild(jumpscareNode);
        rootSelector.AddChild(jumpscareSequence);

        // Branch 2: Chase
        var chaseSequence = new Sequence("Chase Sequence", 1); // Priority trung bình
        var checkPlayerInRangeNode = new CheckPlayerInRange(transform, player, chaseRange, 2f); // Tăng hysteresisRange lên 2f để tránh switch nhanh
        ChasePlayer chaseNode = new ChasePlayer(navAgent, player, animator, "Run", 0, jumpscareRange, chaseRange);
        chaseSequence.AddChild(checkPlayerInRangeNode);
        chaseSequence.AddChild(chaseNode);
        rootSelector.AddChild(chaseSequence);

        // Branch 3: Patrol
        Patrol patrolNode = new Patrol(navAgent, patrolPoints, animator, "Walk", "Idle", 0, true, 0.8f, 1.8f, "Patrol", 0);
        rootSelector.AddChild(patrolNode);

        // Branch 4: Hide behavior
        var hideSequence = new Sequence("Hide Sequence", 0);
        hideSequence.AddChild(new Leaf("Is Hiding Check", new ConditionStrategy(() =>
        {
            Debug.Log($"Is Hiding Check: {isHiding}");
            return isHiding;
        })));
        var hideSelector = new Selector("Hide Options");
        var hiddenChaseSequence = new Sequence("Hidden Chase");
        hiddenChaseSequence.AddChild(new Leaf("Chase Range Check", new ConditionStrategy(() => IsPlayerInChaseRange())));
        hiddenChaseSequence.AddChild(chaseNode);
        hideSelector.AddChild(hiddenChaseSequence);
        hideSelector.AddChild(new Leaf("Look Around", new ActionStrategy(() => LookAround())));
        hideSequence.AddChild(hideSelector);
        rootSelector.AddChild(hideSequence);

        // Branch 5: Health check
        var healthSequence = new Sequence("Health Sequence", 0);
        healthSequence.AddChild(new Leaf("Health Check", new ConditionStrategy(() => CheckHealth())));
        rootSelector.AddChild(healthSequence);

        // Sử dụng phương thức SortChildrenByPriority thay vì truy cập trực tiếp Children
        rootSelector.SortChildrenByPriority();

        behaviorTree.AddChild(rootSelector);
    }

    #region Detection Methods
    private void UpdatePlayerDetection()
    {
        if (player == null)
        {
            Debug.LogWarning("Player is null in UpdatePlayerDetection!");
            return;
        }

        // Sử dụng khoảng cách 3D
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Thêm kiểm tra FOV và Line of Sight (LOS)
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        bool inFOV = angle < fieldOfViewAngle / 2;

        // Kiểm tra LOS (không có vật cản)
        bool hasLOS = !Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Default")); // Thay "Default" bằng layer vật cản nếu có

        if (inFOV && hasLOS)
        {
            isPlayerInJumpscareRange = distanceToPlayer <= jumpscareRange;
            isPlayerInRange = distanceToPlayer > jumpscareRange && distanceToPlayer <= chaseRange;
        }
        else
        {
            isPlayerInJumpscareRange = false;
            isPlayerInRange = false;
        }

        // Debug chi tiết
        //Debug.Log($"DistanceToPlayer={distanceToPlayer:F2}, JumpscareRange={jumpscareRange:F2}, ChaseRange={chaseRange:F2}, InJumpscare={isPlayerInJumpscareRange}, InChase={isPlayerInRange}, InFOV={inFOV}, HasLOS={hasLOS}, PlayerPos={player.position}, EnemyPos={transform.position}");
    }

    private bool IsPlayerInChaseRange()
    {
        bool result = isPlayerInRange;
        Debug.Log($"IsPlayerInChaseRange: {result}");
        return result;
    }

    private bool IsPlayerInJumpscareRange()
    {
        bool result = isPlayerInJumpscareRange;
        Debug.Log($"IsPlayerInJumpscareRange: {result}");
        return result;
    }

    private bool CheckHealth()
    {
        Debug.Log("Health Check: Always true (placeholder)");
        return true; // Placeholder
    }
    #endregion

    #region Action Methods
    private void LookAround()
    {
        transform.Rotate(0, 45f * Time.deltaTime, 0);
        Debug.Log("Looking around...");
    }
    #endregion

    #region Debug
    void OnDrawGizmosSelected()
    {
        if (transform == null) return;

        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw chase range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Draw jumpscare range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, jumpscareRange);

        // Draw FOV
        Gizmos.color = Color.blue;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * transform.forward * chaseRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * transform.forward * chaseRange;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Draw patrol points
        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawWireCube(patrolPoints[i].position, Vector3.one * 0.5f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }
            }
            if (patrolPoints.Length > 1 && patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
            }
        }
    }
    #endregion
}