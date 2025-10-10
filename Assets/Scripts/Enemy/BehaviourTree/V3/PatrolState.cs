using UnityEngine;
using UnityEngine.AI;

public class PatrolState : StateMachineBehaviour
{
    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer; // Gán layer "Wall" trong Inspector

    [Header("Random Patrol Settings")]
    [SerializeField] private float range = 15f; // Bán kính di chuyển ngẫu nhiên
    [SerializeField] private float minWaitTime = 2f; // Thời gian chờ tối thiểu
    [SerializeField] private float maxWaitTime = 5f; // Thời gian chờ tối đa

    [Header("Audio Settings")]
    [SerializeField] private AudioClip breathingSound; // Kéo file âm thanh vào đây
    [SerializeField] private float minTimeBetweenBreaths = 3f; // Thời gian tối thiểu giữa các lần thở
    [SerializeField] private float maxTimeBetweenBreaths = 8f; // Thời gian tối đa
    [SerializeField] private float breathVolume = 0.7f; // Âm lượng (0-1)

    float timer;
    float waitTimer;
    float waitDuration;
    bool isWaiting = false;
    NavMeshAgent agent;
    Transform player;
    Transform centrePoint; // Vị trí trung tâm để wander
    float chaseRange = 10;

    // Audio variables
    private AudioSource audioSource;
    private float nextBreathTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 2.5f;
        timer = 0;

        // Dùng vị trí hiện tại làm centre point
        centrePoint = animator.transform;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ✅ SETUP AUDIO
        SetupAudio(animator);
        ScheduleNextBreath();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ✅ PHÁT ÂM THANH THỞ
        if (Time.time >= nextBreathTime && breathingSound != null)
        {
            PlayBreathingSound();
            ScheduleNextBreath();
        }

        // ✅ Khi đã đến đích → tìm điểm mới
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
            }
        }

        timer += Time.deltaTime;

        // Check for player jumpscare range first
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance <= 2.5f)
        {
            animator.SetBool("IsJumpscaring", true);
            Debug.Log("Direct jumpscare from patrol");
            return;
        }

        if (timer > 10)
            animator.SetBool("IsPatrolling", false);

        // Check line of sight
        if (distance < chaseRange && CanSeePlayer(animator))
        {
            animator.SetBool("IsAlert", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);

        // ✅ KHÔNG DỪNG ÂM THANH - Để âm thanh phát hết tự nhiên
        // AudioSource sẽ tự dừng khi âm thanh phát xong
    }

    // ✅ AUDIO METHODS
    void SetupAudio(Animator animator)
    {
        audioSource = animator.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = animator.gameObject.AddComponent<AudioSource>();
        }

        // Cấu hình 3D sound
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.maxDistance = 20f; // Khoảng cách nghe được
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = breathVolume;
    }

    void PlayBreathingSound()
    {
        if (audioSource != null && breathingSound != null)
        {
            audioSource.PlayOneShot(breathingSound, breathVolume);
        }
    }

    void ScheduleNextBreath()
    {
        nextBreathTime = Time.time + Random.Range(minTimeBetweenBreaths, maxTimeBetweenBreaths);
    }

    // ✅ METHOD TỪ SCRIPT BẠN MƯỢN
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    // Check xem có tường chắn không
    bool CanSeePlayer(Animator animator)
    {
        if (player == null) return false;

        float distance = Vector3.Distance(player.position, animator.transform.position);
        Vector3 direction = (player.position - animator.transform.position).normalized;

        // Raycast từ vị trí mắt Agent
        Vector3 startPos = animator.transform.position + Vector3.up * 1.5f;

        // Raycast để check tường
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, distance, wallLayer))
        {
            // Nếu hit tường trước khi tới Player = có tường chắn
            Debug.DrawRay(startPos, direction * hit.distance, Color.red, 0.1f);
            return false;
        }

        // Không có tường chắn
        Debug.DrawRay(startPos, direction * distance, Color.green, 0.1f);
        return true;
    }
}