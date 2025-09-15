using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{
    public Transform player;
    public float detectRange = 5f;
    public float speed = 2f;
    public Transform[] patrolPoints;
    public Transform camera;
    private btNode root;
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
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    void Start()
    {
        var chase = new ChasePlayer(agent, player, anim, "Run");
        var closeToPlayer = new CloseToPlayer(transform, player, 0.5f);
        var jumpscare = new Jumpscare(agent, anim, transform, Camera.main.transform, "Jumpscare");
        var patrol = new Patrol(agent, patrolPoints, anim, "Walk", "Idle", 0, pauseAtWaypoint: true, 0.8f, 1.8f);

        btNode[] chaseToJumpscareSequence = { chase, closeToPlayer, jumpscare };
        btNode[] arraySelector = { new sequence(chaseToJumpscareSequence), patrol };
        root = new selector(arraySelector);
    }

    void Update()
    {
        root.evaluate();
    }
}