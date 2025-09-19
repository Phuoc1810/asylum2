using UnityEngine;
using UnityEngine.AI;

public class CheckPlayerInRange : Node
{
    private Transform ai;
    private Transform player;
    private float detectRange;
    private float hysteresisRange; // Vùng đệm để tránh nhảy trạng thái
    private bool wasInRange = false; // Theo dõi trạng thái trước đó

    public CheckPlayerInRange(Transform ai, Transform player, float detectRange = 2.0f, float hysteresisRange = 0.5f)
    {
        this.ai = ai;
        this.player = player;
        this.detectRange = detectRange; // Phạm vi phát hiện mặc định 2m
        this.hysteresisRange = hysteresisRange; // Vùng đệm mặc định 0.5m
    }

    public override NodeState Evaluate()
    {
        if (ai == null || player == null)
            return NodeState.Failure;

        float dist = Vector3.Distance(ai.position, player.position);
        float threshold = wasInRange ? detectRange + hysteresisRange : detectRange; // Thêm vùng đệm khi đã phát hiện

        if (dist <= threshold)
        {
            wasInRange = true;
            State = NodeState.Success;
        }
        else
        {
            wasInRange = false;
            State = NodeState.Failure;
        }
        return State;
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
    private float lostSightTimer = 0f; // Timer để "mất dấu" player
    private float lostSightTime = 1.5f; // Sau 1.5s mất dấu, fallback Failure
    private float lastSeenDistance = float.MaxValue; // Khoảng cách lần cuối thấy player
    private float detectRange; // Lấy từ CheckPlayerInRange hoặc hardcode

    public ChasePlayer(
        NavMeshAgent agent,
        Transform player,
        Animator animator = null,
        string runAnim = "Run",
        int animLayer = 0,
        float stoppingDistance = 0.5f,
        float detectRange = 2.0f // Truyền detectRange để đồng bộ
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

    public override NodeState Evaluate()
    {
        if (agent == null || player == null || !agent.enabled)
            return NodeState.Failure;

        if (animator != null && !playedRunOnce)
        {
            PlayIfNotCurrent(runAnim);
            playedRunOnce = true;
        }

        float currentDistance = Vector3.Distance(agent.transform.position, player.position);
        lastSeenDistance = currentDistance;

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f || currentDistance > stoppingDistance * 2)
        {
            if (NavMesh.SamplePosition(player.position, out var hit, 2.0f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
            repathTimer = repathInterval;
        }

        if (currentDistance <= stoppingDistance)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            return NodeState.Success;
        }

        // Logic lost sight: Nếu player ra ngoài detectRange quá lâu, fallback Failure
        if (currentDistance > detectRange)
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer > lostSightTime)
            {
                Debug.Log("Lost sight of player, fallback to patrol");
                agent.isStopped = true; // Dừng agent khi mất dấu
                return NodeState.Failure; // Fallback sang patrol
            }
        }
        else
        {
            lostSightTimer = 0f; // Reset nếu thấy player
        }

        agent.isStopped = false;
        return NodeState.Running;
    }

    private void PlayIfNotCurrent(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return;

        var st = animator.GetCurrentAnimatorStateInfo(animLayer);
        if (!st.IsName(stateName))
            animator.Play(stateName, animLayer, 0f);
    }
}