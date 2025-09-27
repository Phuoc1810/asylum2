using UnityEngine;
using UnityEngine.AI;

public class Chase : StateMachineBehaviour
{
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

        // Cập nhật đích đến là vị trí người chơi
        agent.SetDestination(player.position);

        // PRIORITY 1: Kiểm tra jumpscare
        if (distance <= jumpscareRange)
        {
            animator.SetBool("IsJumpscaring", true);
            return;
        }

        // Quản lý timer mất tích người chơi
        if (distance > losePlayerRange)
        {
            lostPlayerTimer += Time.deltaTime;
            // Chỉ thoát chase sau 3 giây mất tích
            if (lostPlayerTimer > 3f)
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
}