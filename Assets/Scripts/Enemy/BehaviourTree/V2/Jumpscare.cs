using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class CloseToPlayer : Node
{
    private Transform ai;
    private Transform player;
    private float closeRange;

    public CloseToPlayer(Transform ai, Transform player, float closeRange = 0.5f) // Default 0.5m cho jumpscare
    {
        this.ai = ai;
        this.player = player;
        this.closeRange = closeRange;
    }

    public override NodeState Evaluate()
    {
        if (ai == null || player == null)
            return NodeState.Failure;

        float dist = Vector3.Distance(ai.position, player.position);
        State = dist <= closeRange ? NodeState.Success : NodeState.Failure;
        return State;
    }
}
public class Jumpscare : Node
{
    private Transform player;
    private Transform enemy;
    private Animator animator;
    private string jumpscareTrigger;
    private Transform playerCamera;
    private float rotationTime;
    private float rotationElapsed = 0f;
    private bool started = false;

    public Jumpscare(Transform player, Transform enemy, Animator animator, Transform playerCamera, string jumpscareTrigger = "Jumpscare", float rotationTime = 0.3f)
    {
        this.player = player;
        this.enemy = enemy;
        this.animator = animator;
        this.jumpscareTrigger = jumpscareTrigger;
        this.playerCamera = playerCamera;
        this.rotationTime = rotationTime;
    }

    public override NodeState Evaluate()
    {
        if (!started)
        {
            animator.Play(jumpscareTrigger);
            // Vô hiệu hóa CharacterController của player
            if (player != null)
            {
                CharacterController controller = player.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    Debug.Log("Player locked during jumpscare");
                }
                else
                {
                    Debug.LogWarning("No CharacterController found on player");
                }
            }
            started = true;
            rotationElapsed = 0f;
            return NodeState.Running;
        }

        if (rotationElapsed < rotationTime)
        {
            rotationElapsed += Time.deltaTime;
            Vector3 lookDir = (enemy.position - playerCamera.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, rotationElapsed / rotationTime);
            return NodeState.Running;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(jumpscareTrigger))
        {
            return NodeState.Running;
        }

        // Kết thúc jumpscare, kích hoạt lại CharacterController
        if (rotationElapsed >= rotationTime && !animator.GetCurrentAnimatorStateInfo(0).IsName(jumpscareTrigger))
        {
            if (player != null)
            {
                CharacterController controller = player.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = true;
                    Debug.Log("Player unlocked after jumpscare");
                }
            }
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}