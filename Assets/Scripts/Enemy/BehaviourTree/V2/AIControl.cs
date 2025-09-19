using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AIControl : MonoBehaviour
{
    [Header("Player Detection")]
    public Transform player;
    public float detectRange = 2.0f; // Giảm phạm vi phát hiện xuống 2m

    [Header("Movement")]
    public float speed = 2f;
    public Transform[] patrolPoints;

    [Header("Jumpscare")]
    public Transform camera;

    private Node root;
    private NavMeshAgent agent;
    private Animator anim;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (agent == null)
            Debug.LogError("Thiếu NavMeshAgent trên AI!");
        else
        {
            agent.stoppingDistance = 0.5f;
            agent.autoBraking = true;
            agent.speed = speed;
            agent.acceleration = 15f;
        }
    }

    void Start()
    {
        // Validate components
        if (player == null)
        {
            Debug.LogError("Player transform not assigned!");
            return;
        }

        if (camera == null)
        {
            Debug.LogError("Camera transform not assigned!");
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned!");
            return;
        }

        // Create behavior nodes
        var checkPlayer = new CheckPlayerInRange(transform, player, detectRange);
        var chase = new ChasePlayer(agent, player, anim, "Run", 0, 0.5f, detectRange);
        var closeToPlayer = new CloseToPlayer(transform, player, 0.5f);
        var jumpscare = new Jumpscare(player, transform, anim, camera, "Jumpscare");
        var patrol = new Patrol(agent, patrolPoints, anim, "Walk", "Idle", 0, pauseAtWaypoint: true, 0.8f, 1.8f);

        // Build behavior tree: Check if player is in range -> Chase -> Get close -> Jumpscare, otherwise Patrol
        Node[] chaseSequence = { checkPlayer, chase, closeToPlayer, jumpscare };
        var sequenceAction = new Sequence(new List<Node>(chaseSequence));

        Node[] arraySelector = { sequenceAction, patrol };
        root = new Selector(new List<Node>(arraySelector));
    }

    void Update()
    {
        if (root != null)
            root.Evaluate();
        else
            Debug.LogError("Root node is null in AIControl!");
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        if (transform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);
        }
    }
}