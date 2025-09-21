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
    }
    void Start()
    {
        var checkPlayer = new CheckPlayerInRange(transform, player, detectRange);
        var chase = new ChasePlayer(agent, player, anim, "Run");
        var patrol = new Patrol(agent, patrolPoints, anim, "Walk", "Idle", 0, pauseAtWaypoint: true, 0.8f, 1.8f);
        var closeToPlayer = new CloseToPlayer(transform, player, 1.5f);
        var jumpscare = new Jumpscare(
            GetComponent<Animator>(),
            transform,
            Camera.main.transform, // camera player
            "Jumpscare"
        );
        //nhóm nhỏ dùng để gọi hành động
        btNode[] arrayAction = {checkPlayer, chase};
        var sequenceAction = new sequence(arrayAction);
        btNode[] arrayAction2 = { closeToPlayer, jumpscare };
        var sequenceAction2 = new sequence(arrayAction2);
       //nhóm tổng để check tất cả hành động
        btNode[] arraySelector = { sequenceAction2,sequenceAction,patrol };
        root = new selector(arraySelector);

    }

    void Update()
    {
        root.evaluate();
    }
}
