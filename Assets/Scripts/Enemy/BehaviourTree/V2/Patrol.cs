using UnityEngine;
using UnityEngine.AI;

public class Patrol : Node
{
    private NavMeshAgent agent;
    private Transform[] points;

    private Animator animator;
    private string walkAnim;
    private string idleAnim;
    private int animLayer;

    private int index = -1;
    private bool started = false;

    // ---- Tùy chọn chống "chớp" ----
    private float arriveSpeedThreshold = 0.05f; // Tốc độ gần như đứng yên mới coi là tới
    private float hysteresisTime = 0.05f; // Cần đứng yên ít khung hình (chống jitter)
    private float arrivedStillTime = 0f;

    // ---- (Tùy chọn) dừng/Idle ngẫu nhiên tại waypoint ----
    private bool pauseAtWaypoint = false; // Để mặc định false: không phát Idle ở góc
    private float pauseMin = 0.8f, pauseMax = 1.8f;
    private float pauseEndTime = 0f;
    private bool isPaused = false;

    public Patrol(
        NavMeshAgent agent,
        Transform[] points,
        Animator animator,
        string walkAnim = "Walk",
        string idleAnim = "Idle",
        int animLayer = 0,
        bool pauseAtWaypoint = false, // ← Bật nếu bạn muốn dừng
        float pauseMin = 0.8f,
        float pauseMax = 1.8f
    )
    {
        this.agent = agent;
        this.animator = animator;
        this.walkAnim = walkAnim;
        this.idleAnim = idleAnim;
        this.animLayer = animLayer;

        this.pauseAtWaypoint = pauseAtWaypoint;
        this.pauseMin = Mathf.Max(0f, pauseMin);
        this.pauseMax = Mathf.Max(this.pauseMin, pauseMax);

        // Lọc null waypoint và validate NavMesh positions
        if (points != null)
        {
            var list = new System.Collections.Generic.List<Transform>();
            foreach (var p in points)
            {
                if (p != null)
                {
                    // Check if point is on NavMesh
                    if (NavMesh.SamplePosition(p.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                    {
                        list.Add(p);
                    }
                    else
                    {
                        Debug.LogWarning($"Patrol point {p.name} is not on NavMesh, skipping...");
                    }
                }
            }
            this.points = list.ToArray();
        }
        else
        {
            this.points = System.Array.Empty<Transform>();
        }
    }

    public override NodeState Evaluate()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh || points == null || points.Length == 0)
        {
            State = NodeState.Failure;
            return State;
        }

        if (!started)
        {
            index = 0;
            SetNext();
            started = true;
            PlayIfNotCurrent(walkAnim);
            State = NodeState.Running;
            return State;
        }

        // Đang pause tại waypoint?
        if (isPaused)
        {
            if (Time.time >= pauseEndTime)
            {
                isPaused = false;
                if (agent.isStopped) agent.isStopped = false;
                NextPointAndGo();
            }
            State = NodeState.Running;
            return State;
        }

        // Check for path calculation issues
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning("Invalid path in patrol, trying next point");
            NextPointAndGo();
            State = NodeState.Running;
            return State;
        }

        // Điều kiện coi là "đã đến" (có hysteresis chống nháy)
        bool closeEnough = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        if (closeEnough)
        {
            // Nếu tốc độ nhỏ hơn ngưỡng trong một khoảng rất ngắn → thật sự tới
            if (agent.velocity.sqrMagnitude < arriveSpeedThreshold * arriveSpeedThreshold)
            {
                arrivedStillTime += Time.deltaTime;
                if (arrivedStillTime >= hysteresisTime)
                {
                    if (pauseAtWaypoint)
                    {
                        // Pause: dừng và Idle
                        isPaused = true;
                        pauseEndTime = Time.time + Random.Range(pauseMin, pauseMax);
                        if (!agent.isStopped) agent.isStopped = true;
                        PlayIfNotCurrent(idleAnim);
                        State = NodeState.Running;
                        return State;
                    }
                    else
                    {
                        // Không pause: chuyển waypoint ngay, KHÔNG phát Idle để tránh nháy
                        NextPointAndGo();
                        State = NodeState.Running;
                        return State;
                    }
                }
            }
            else
            {
                // Còn đang quay/nhích → reset bộ đếm
                arrivedStillTime = 0f;
            }
        }
        else
        {
            arrivedStillTime = 0f;
        }

        // Đang di chuyển → giữ Walk
        if (agent.velocity.sqrMagnitude > 0.01f)
            PlayIfNotCurrent(walkAnim);

        State = NodeState.Running;
        return State;
    }

    private void NextPointAndGo()
    {
        if (points.Length == 0) return;

        index = (index + 1) % points.Length;
        SetNext();
        if (agent.isStopped) agent.isStopped = false;
        PlayIfNotCurrent(walkAnim);
        arrivedStillTime = 0f; // Reset arrival timer
    }

    private void SetNext()
    {
        if (points.Length == 0 || index < 0 || index >= points.Length) return;

        var target = points[index].position;
        if (NavMesh.SamplePosition(target, out var hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // If current point fails, try next point
            Debug.LogWarning($"NavMesh sample failed for patrol point {index}, trying next point");
            index = (index + 1) % points.Length;
            if (NavMesh.SamplePosition(points[index].position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.LogError("All patrol points seem to be invalid!");
            }
        }
    }

    private void PlayIfNotCurrent(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return;

        // Check if animation exists
        if (!animator.HasState(animLayer, Animator.StringToHash(stateName)))
        {
            Debug.LogWarning($"Animation state '{stateName}' not found in animator!");
            return;
        }

        var st = animator.GetCurrentAnimatorStateInfo(animLayer);
        if (!st.IsName(stateName))
            animator.Play(stateName, animLayer, 0f);
    }
}