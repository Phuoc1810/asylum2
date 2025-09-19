using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AIControl : MonoBehaviour
{
    public Transform player;
    public float detectRange = 2.0f; // Giảm phạm vi phát hiện xuống 2m
    public float speed = 2f;
    public Transform[] patrolPoints;
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
        var checkPlayer = new CheckPlayerInRange(transform, player, detectRange);
        var chase = new ChasePlayer(agent, player, anim, "Run", 0, 0.5f, detectRange); // Thêm detectRange
        var patrol = new Patrol(agent, patrolPoints, anim, "Walk", "Idle", 0, pauseAtWaypoint: true, 0.8f, 1.8f);
        var closeToPlayer = new CloseToPlayer(transform, player, 0.5f);
        var jumpscare = new Jumpscare(player, transform, anim, camera, "Jumpscare");

        Node[] chaseSequence = { chase, closeToPlayer, jumpscare };
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
}