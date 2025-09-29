using UnityEngine;
using UnityEngine.AI;

public class AlertState : StateMachineBehaviour
{
    float timer;
    Transform player;
    NavMeshAgent agent;
    float chaseRange = 20f;
    float losePlayerRange = 20f;
    float alertDuration = 3f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 1f;
        agent.SetDestination(agent.transform.position);
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Debug.Log("Alert State Started");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (player == null) return;

        float distance = Vector3.Distance(player.position, animator.transform.position);

        // Jumpscare nếu người chơi đến quá gần
        if (distance <= 2.5f)
        {
            animator.SetBool("IsJumpscaring", true);
            Debug.Log("Direct jumpscare from alert");
            return;
        }

        // Quay mặt về phía người chơi
        Vector3 direction = (player.position - animator.transform.position).normalized;
        animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation,
            Quaternion.LookRotation(direction), Time.deltaTime * 2f);

        // Chuyển sang chase nếu đã alert đủ lâu
        if (distance < chaseRange && timer > 1f)
        {
            animator.SetBool("IsRunning", true);
            Debug.Log("Switching to chase from alert");
            return;
        }

        // Mất tích người chơi hoặc hết thời gian alert
        if (distance > losePlayerRange || timer > alertDuration)
        {
            animator.SetBool("IsAlert", false);
            Debug.Log("Alert lost player, returning to idle");
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}