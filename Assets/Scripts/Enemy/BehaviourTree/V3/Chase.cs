using UnityEngine;
using UnityEngine.AI;

public class Chase : StateMachineBehaviour
{
    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer; // Gán layer "Wall" trong Inspector

    [Header("Sound Effects")]
    [SerializeField] private AudioClip chaseSound; // Âm thanh chase/chạy
    [SerializeField] private AudioClip footstepSound; // Âm thanh bước chân (optional)
    [SerializeField] private float footstepInterval = 0.5f; // Khoảng cách giữa các tiếng bước

    NavMeshAgent agent;
    Transform player;
    AudioSource audioSource;
    float chaseSpeed = 4f;
    float losePlayerRange = 15f;
    float jumpscareRange = 2.5f;
    float timer;
    float lostPlayerTimer; // Timer riêng cho việc mất tích
    float maxChaseTime = 30f; // Thời gian chase tối đa
    float footstepTimer; // Timer cho tiếng bước chân

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timer = 0;
        lostPlayerTimer = 0;
        footstepTimer = 0;

        // Lấy hoặc thêm AudioSource component
        audioSource = animator.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = animator.gameObject.AddComponent<AudioSource>();
        }

        // Phát âm thanh chase (looping background)
        if (chaseSound != null)
        {
            audioSource.clip = chaseSound;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.minDistance = 5f;
            audioSource.maxDistance = 20f;
            audioSource.Play();
        }
        // ✅ TẮT CÁC BOOL CỦA STATE KHÁC
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAlert", false);
        animator.SetBool("IsPatrolling", false); // Nếu có
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        float distance = Vector3.Distance(player.position, animator.transform.position);

        // PRIORITY 1: Kiểm tra jumpscare
        if (distance <= jumpscareRange)
        {
            animator.SetBool("IsJumpscaring", true);
            return;
        }

        // ✅ CHECK LINE OF SIGHT
        bool canSeePlayer = CanSeePlayer(animator);

        // Cập nhật đích đến là vị trí người chơi
        agent.SetDestination(player.position);

        // ✅ PHÁT TIẾNG BƯỚC CHÂN (nếu có)
        if (footstepSound != null && agent.velocity.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                AudioSource.PlayClipAtPoint(footstepSound, animator.transform.position, 0.5f);
                footstepTimer = 0;
            }
        }

        // ✅ QUẢN LÝ TIMER MẤT TÍCH - nhanh hơn khi không nhìn thấy
        if (distance > losePlayerRange || !canSeePlayer)
        {
            // Nếu KHÔNG nhìn thấy player, mất tích nhanh hơn (1.5 giây thay vì 3 giây)
            float lostTimeThreshold = canSeePlayer ? 3f : 1.5f;

            lostPlayerTimer += Time.deltaTime;

            // Chỉ thoát chase sau khi mất tích đủ lâu
            if (lostPlayerTimer > lostTimeThreshold)
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsAlert", false);
                return;
            }
        }
        else
        {
            // Reset lost timer khi tìm thấy người chơi lại
            lostPlayerTimer = 0;
        }

        // PRIORITY 3: Timeout tổng thể
        if (timer > maxChaseTime)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAlert", false);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);

        // Dừng âm thanh chase khi thoát state
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
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