using UnityEngine;
using UnityEngine.AI;

public class CheckPlayerInRange : Node
{
    private Transform ai;
    private Transform player;
    private float detectRange;
    private float hysteresisRange;
    private bool wasInRange = false;

    public CheckPlayerInRange(Transform ai, Transform player, float detectRange = 15f, float hysteresisRange = 0.5f)
    {
        this.ai = ai;
        this.player = player;
        this.detectRange = detectRange;
        this.hysteresisRange = hysteresisRange;
    }

    public override NodeStatus Process()
    {
        if (ai == null || player == null)
        {
            Debug.LogWarning("CheckPlayerInRange: AI or Player is null");
            CurrentStatus = NodeStatus.Failure;
            return CurrentStatus;
        }

        float dist = Vector3.Distance(ai.position, player.position);
        float threshold = wasInRange ? detectRange + hysteresisRange : detectRange;

        if (dist <= threshold)
        {
            wasInRange = true;
            CurrentStatus = NodeStatus.Success;
        }
        else
        {
            wasInRange = false;
            CurrentStatus = NodeStatus.Failure;
        }
        Debug.Log($"CheckPlayerInRange: Distance={dist:F2}, Threshold={threshold:F2}, Result={CurrentStatus}");
        return CurrentStatus;
    }
}

public class ChasePlayer : Node
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private string runAnim;
    private int animLayer;
    private float stoppingDistance;
    private bool playedRunOnce = false;
    private float repathInterval = 0.1f;
    private float repathTimer = 0f;
    private float lostSightTimer = 0f;
    private float lostSightTime = 2f; // Tăng lên 2f để giữ chase lâu hơn nếu player di chuyển ra ngoài nhẹ
    private float detectRange;

    public ChasePlayer(
        NavMeshAgent agent,
        Transform player,
        Animator animator = null,
        string runAnim = "Run",
        int animLayer = 0,
        float stoppingDistance = 5f, // Đồng bộ với jumpscareRange
        float detectRange = 15f
    )
    {
        this.agent = agent;
        this.player = player;
        this.animator = animator;
        this.runAnim = runAnim;
        this.animLayer = animLayer;
        this.stoppingDistance = stoppingDistance;
        this.detectRange = detectRange;
    }

    public override NodeStatus Process()
    {
        
        if (agent == null || player == null || !agent.enabled || !agent.isOnNavMesh)
        {
            //Debug.LogWarning($"ChasePlayer failed: Agent={agent}, Player={player}, Enabled={agent?.enabled}, OnNavMesh={agent?.isOnNavMesh}");
            CurrentStatus = NodeStatus.Failure;
            return CurrentStatus;
        }

        if (!playedRunOnce)
        {
            PlayIfNotCurrent(runAnim);
            agent.SetDestination(player.position);
            playedRunOnce = true;
            lostSightTimer = 0f;
            agent.speed = 3.5f; // Đặt tốc độ đuổi
            Debug.Log("Chase started");
        }

        float currentDistance = Vector3.Distance(agent.transform.position, player.position);

        // Check if too close (in jumpscare range)
        if (currentDistance <= stoppingDistance)
        {
            Debug.Log("ChasePlayer: Within jumpscare range, stopping to allow jumpscare");
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            CurrentStatus = NodeStatus.Success;
            return CurrentStatus;
        }

        // Update destination periodically
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f || currentDistance > stoppingDistance * 1.5f)
        {
            if (NavMesh.SamplePosition(player.position, out var hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log($"ChasePlayer: Set destination to {hit.position}");
            }
            else
            {
                Debug.LogWarning("Cannot find valid NavMesh position near player");
            }
            repathTimer = repathInterval;
        }

        // Logic lost sight
        if (currentDistance > detectRange)
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer > lostSightTime)
            {
                Debug.Log("ChasePlayer: Lost sight of player, stopping chase");
                agent.isStopped = true;
                playedRunOnce = false;
                CurrentStatus = NodeStatus.Failure;
                return CurrentStatus;
            }
        }
        else
        {
            lostSightTimer = 0f;
        }

        agent.isStopped = false;
        CurrentStatus = NodeStatus.Running;
        Debug.Log($"ChasePlayer: Running, Distance={currentDistance:F2}, Destination={agent.destination}");
        return CurrentStatus;
    }

    private void PlayIfNotCurrent(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return;

        var st = animator.GetCurrentAnimatorStateInfo(animLayer);
        if (!st.IsName(stateName))
        {
            animator.Play(stateName, animLayer, 0f);
            Debug.Log($"Playing animation: {stateName}");
        }
    }
}