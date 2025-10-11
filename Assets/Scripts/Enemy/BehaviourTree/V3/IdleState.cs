using UnityEngine;
using UnityEngine.AI;

public class IdleState : StateMachineBehaviour
{
    [Header("Idle Settings")]
    [SerializeField] private float idleDuration = 5f;
    [SerializeField] private float detectionRange = 10f;

    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer; // Gán layer "Wall" trong Inspector

    [Header("Audio Settings")]
    [SerializeField] private AudioClip breathingSound; // Kéo file âm thanh vào đây
    [SerializeField] private float minTimeBetweenBreaths = 4f; // Thời gian tối thiểu giữa các lần thở
    [SerializeField] private float maxTimeBetweenBreaths = 10f; // Thời gian tối đa (idle thở chậm hơn patrol)
    [SerializeField] private float breathVolume = 0.6f; // Âm lượng (0-1)

    private float timer;
    private Transform player;
    private NavMeshAgent agent;

    // Audio variables
    private AudioSource audioSource;
    private float nextBreathTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset timer
        timer = 0f;

        // Get components
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Stop agent movement
        if (agent != null)
        {
            agent.SetDestination(agent.transform.position);
            agent.speed = 0f;
            agent.isStopped = true;
        }

        // ✅ SETUP AUDIO
        SetupAudio(animator);
        ScheduleNextBreath();

        Debug.Log("Entered Idle State");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ✅ PHÁT ÂM THANH THỞ
        if (Time.time >= nextBreathTime && breathingSound != null)
        {
            PlayBreathingSound();
            ScheduleNextBreath();
        }

        timer += Time.deltaTime;

        // Check for player jumpscare range first
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, animator.transform.position);

            if (distanceToPlayer <= 2.5f)
            {
                animator.SetBool("IsJumpscaring", true);
                Debug.Log("Direct jumpscare from idle");
                return;
            }

            // ✅ THÊM CHECK LINE OF SIGHT
            if (distanceToPlayer <= detectionRange && CanSeePlayer(animator))
            {
                animator.SetBool("IsAlert", true);
                return;
            }
        }

        // Transition to patrol after idle duration
        if (timer >= idleDuration)
        {
            animator.SetBool("IsPatrolling", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Re-enable agent when leaving idle
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = 2.5f;
        }

        // ✅ KHÔNG DỪNG ÂM THANH - Để âm thanh phát hết tự nhiên
        // AudioSource sẽ tự dừng khi âm thanh phát xong

        Debug.Log("Exited Idle State");
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

    // ✅ METHOD MỚI: Check xem có tường chắn không
    bool CanSeePlayer(Animator animator)
    {
        if (player == null) return false;

        float distance = Vector3.Distance(player.position, animator.transform.position);
        Vector3 direction = (player.position - animator.transform.position).normalized;

        // Raycast từ vị trí mắt Agent (thêm Vector3.up để tránh hit ground)
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