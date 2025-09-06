using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
public interface IState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Exit();
    void Enter();
    void Execute();
}
public class PatrolState : IState
{
    private NavMeshAgent agent;
    private List<Transform> patrolPoints;
    private int currentIndex = 0;
    private StateMachine stateMachine;
    private Transform player;
    private float detectionRange;

    public PatrolState(StateMachine sm, NavMeshAgent agent, List<Transform> patrolPoints, Transform player, float detectionRange)
    {
        this.stateMachine = sm;
        this.agent = agent;
        this.patrolPoints = patrolPoints;
        this.player = player;
        this.detectionRange = detectionRange;
    }

    public void Enter()
    {
        if (patrolPoints.Count > 0)
            agent.SetDestination(patrolPoints[currentIndex].position);
    }

    public void Execute()
    {
        if (Vector3.Distance(agent.transform.position, player.position) < detectionRange)
        {
            stateMachine.ChangeState(new DetectPlayerState(stateMachine, agent, player));
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentIndex = (currentIndex + 1) % patrolPoints.Count;
            agent.SetDestination(patrolPoints[currentIndex].position);
        }
    }

    public void Exit() { }
}

// ========== DETECT PLAYER STATE ========== //
public class DetectPlayerState : IState
{
    private NavMeshAgent agent;
    private Transform player;
    private StateMachine stateMachine;
    private float detectionTime = 1.5f;
    private float timer = 0f;

    public DetectPlayerState(StateMachine sm, NavMeshAgent agent, Transform player)
    {
        this.stateMachine = sm;
        this.agent = agent;
        this.player = player;
    }

    public void Enter()
    {
        timer = 0f;
        agent.isStopped = true;
        Debug.Log("🔍 Player Detected!");
    }

    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= detectionTime)
        {
            stateMachine.ChangeState(new ChaseState(stateMachine, agent, player, 1.5f));
        }
    }

    public void Exit()
    {
        agent.isStopped = false;
    }
}

// ========== CHASE STATE ========== //
public class ChaseState : IState
{
    private NavMeshAgent agent;
    private Transform player;
    private StateMachine stateMachine;
    private float chaseSpeed;
    private float catchDistance = 1.5f;

    public ChaseState(StateMachine sm, NavMeshAgent agent, Transform player, float speed)
    {
        this.stateMachine = sm;
        this.agent = agent;
        this.player = player;
        this.chaseSpeed = speed;
    }

    public void Enter()
    {
        Debug.Log("🏃‍♂️ Chasing the player...");
        agent.speed = chaseSpeed;
    }

    public void Execute()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        if (Vector3.Distance(agent.transform.position, player.position) < catchDistance)
        {
            stateMachine.ChangeState(new JumpscareState(stateMachine, agent));
        }
    }

    public void Exit() { }
}

// ========== JUMPSCARE STATE ========== //
public class JumpscareState : IState
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private float timer = 0f;
    private float jumpscareDuration = 2f;

    public JumpscareState(StateMachine sm, NavMeshAgent agent)
    {
        this.stateMachine = sm;
        this.agent = agent;
    }

    public void Enter()
    {
        Debug.Log("💀 BOO!");
        agent.isStopped = true;

        // Thêm animation, hiệu ứng, âm thanh tại đây
    }

    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer > jumpscareDuration)
        {
            Debug.Log("Game over / reset...");
            // SceneManager.LoadScene(...) hoặc reset trạng thái
        }
    }

    public void Exit() { }
}