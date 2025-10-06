using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class PatrolStrategy : IStrategy
{
    private NavMeshAgent agent;
    private Transform[] points;
    private Animator animator;
    private string walkAnim;
    private string idleAnim;
    private int animLayer;

    private int index = -1;
    private bool started = false;

    // Anti-jitter options
    private float arriveSpeedThreshold = 0.05f; // Speed near zero to consider arrived
    private float hysteresisTime = 0.05f;       // Still time required to confirm arrival
    private float arrivedStillTime = 0f;

    // Optional pause at waypoint
    private bool pauseAtWaypoint = false;
    private float pauseMin = 0.8f, pauseMax = 1.8f;
    private float pauseEndTime = 0f;
    private bool isPaused = false;

    public PatrolStrategy(
        NavMeshAgent agent,
        Transform[] points,
        Animator animator = null,
        string walkAnim = "Walk",
        string idleAnim = "Idle",
        int animLayer = 0,
        bool pauseAtWaypoint = false,
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

        // Filter null points
        this.points = points?.Where(p => p != null).ToArray() ?? new Transform[0];
    }

    public NodeStatus Process()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh || points.Length == 0)
            return NodeStatus.Failure;

        if (!started)
        {
            index = 0;
            SetNext();
            started = true;
            PlayIfNotCurrent(walkAnim);
            return NodeStatus.Running;
        }

        // Paused at waypoint?
        if (isPaused)
        {
            if (Time.time >= pauseEndTime)
            {
                isPaused = false;
                if (agent.isStopped) agent.isStopped = false;
                NextPointAndGo();
            }
            return NodeStatus.Running;
        }

        // Check arrival with hysteresis to avoid jitter
        bool closeEnough = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        if (closeEnough)
        {
            if (agent.velocity.sqrMagnitude < arriveSpeedThreshold * arriveSpeedThreshold)
            {
                arrivedStillTime += Time.deltaTime;
                if (arrivedStillTime >= hysteresisTime)
                {
                    if (pauseAtWaypoint)
                    {
                        // Pause: stop and play idle
                        isPaused = true;
                        pauseEndTime = Time.time + UnityEngine.Random.Range(pauseMin, pauseMax);
                        if (!agent.isStopped) agent.isStopped = true;
                        PlayIfNotCurrent(idleAnim);
                        return NodeStatus.Running;
                    }
                    else
                    {
                        // No pause: go to next immediately, no idle to avoid flicker
                        NextPointAndGo();
                        return NodeStatus.Running;
                    }
                }
            }
            else
            {
                // Still adjusting/moving slightly, reset counter
                arrivedStillTime = 0f;
            }
        }
        else
        {
            arrivedStillTime = 0f;
        }

        // If moving, ensure walk animation
        if (agent.velocity.sqrMagnitude > 0.01f)
            PlayIfNotCurrent(walkAnim);

        return NodeStatus.Running;
    }

    public void Reset()
    {
        index = -1;
        started = false;
        arrivedStillTime = 0f;
        isPaused = false;
        pauseEndTime = 0f;
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = false;
        }
    }

    private void NextPointAndGo()
    {
        index = (index + 1) % points.Length;
        SetNext();
        if (agent.isStopped) agent.isStopped = false;
        PlayIfNotCurrent(walkAnim);
    }

    private void SetNext()
    {
        Vector3 target = points[index].position;
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    private void PlayIfNotCurrent(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return;

        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(animLayer);
        if (!st.IsName(stateName))
            animator.Play(stateName, animLayer, 0f);
    }
}

public class Patrol : Node
{
    public Patrol(
        NavMeshAgent agent,
        Transform[] points,
        Animator animator = null,
        string walkAnim = "Walk",
        string idleAnim = "Idle",
        int animLayer = 0,
        bool pauseAtWaypoint = false,
        float pauseMin = 0.8f,
        float pauseMax = 1.8f,
        string name = "Patrol",
        int priority = 0
    ) : base(name, priority)
    {
        IStrategy strategy = new PatrolStrategy(
            agent, points, animator, walkAnim, idleAnim, animLayer,
            pauseAtWaypoint, pauseMin, pauseMax
        );
        AddChild(new Leaf(name, strategy, priority));
    }

    public override NodeStatus Process()
    {
        return base.Process();
    }
}