using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : Node
{
    private Transform playerTransform;
    private Transform enemyTransform;
    private NavMeshAgent agent;

    public ChasePlayer(Transform playerTransform, Transform enemyTransform, NavMeshAgent agent)
    {
        this.playerTransform = playerTransform;
        this.enemyTransform = enemyTransform;
        this.agent = agent;
    }
    public override NodeState Evaluate()
    {
        agent.SetDestination(playerTransform.position);
        agent.isStopped = false;
        State = NodeState.Running;
        return State;
    }
}
