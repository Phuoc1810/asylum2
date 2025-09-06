using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.Android.Gradle;
public class EnemyStateManager : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 8f;
    public float catchDistance = 1.5f;


    private StateMachine stateMachine;
    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player không được tìm thấy! Đảm bảo rằng Player có tag là 'Player'.");
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("Chưa gán patrolPoints cho NPC.");
            return;
        }

        List<Transform> patrolList = new List<Transform> (patrolPoints);
        IState patrol = new PatrolState(stateMachine, agent, patrolList, player, detectionRange);
        stateMachine.ChangeState(patrol);
    }

    void Update()
    {
        stateMachine.Update(); // Gọi Execute của state hiện tại
    }
}

