using UnityEngine;
using UnityEngine.AI;

public class Chase : StateMachineBehaviour
{
    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer; // Gán layer "Wall" trong Inspector

    NavMeshAgent agent;
    Transform player;
    float chaseSpeed = 4f;
    float losePlayerRange = 15f;
    float jumpscareRange = 2.5f;
    float timer;
    float lostPlayerTimer; // Timer riêng cho việc mất tích
    float maxChaseTime = 30f; // Thời gian chase tối đa

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timer = 0;
        lostPlayerTimer = 0;

        // Có thể thêm âm thanh chase ở đây
        // AudioSource.PlayClipAtPoint(chaseSound, animator.transform.position);
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