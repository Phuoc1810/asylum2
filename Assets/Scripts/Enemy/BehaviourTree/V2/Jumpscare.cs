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
        {
            State = NodeState.Failure;
            return State;
        }

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
    private bool playerLocked = false;
    private float jumpscareTimeout = 5f; // Timeout để tránh stuck
    private float jumpscareTimer = 0f;

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
        // Safety checks
        if (player == null || enemy == null || animator == null || playerCamera == null)
        {
            State = NodeState.Failure;
            return State;
        }

        if (!started)
        {
            // Start jumpscare
            if (animator.HasState(0, Animator.StringToHash(jumpscareTrigger)))
            {
                animator.Play(jumpscareTrigger);
            }
            else
            {
                Debug.LogError($"Animation state '{jumpscareTrigger}' not found in animator!");
                State = NodeState.Failure;
                return State;
            }

            // Lock player movement
            LockPlayer();

            started = true;
            rotationElapsed = 0f;
            jumpscareTimer = 0f;
            State = NodeState.Running;
            return State;
        }

        // Update timers
        jumpscareTimer += Time.deltaTime;

        // Timeout safety
        if (jumpscareTimer > jumpscareTimeout)
        {
            Debug.LogWarning("Jumpscare timeout reached, ending jumpscare");
            UnlockPlayer();
            ResetJumpscare();
            State = NodeState.Success;
            return State;
        }

        // Handle camera rotation during jumpscare
        if (rotationElapsed < rotationTime)
        {
            rotationElapsed += Time.deltaTime;
            Vector3 lookDir = (enemy.position - playerCamera.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            float t = Mathf.Clamp01(rotationElapsed / rotationTime);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, t);

            State = NodeState.Running;
            return State;
        }

        // Check if animation is still playing
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingJumpscare = stateInfo.IsName(jumpscareTrigger);
        bool animationComplete = stateInfo.normalizedTime >= 1.0f;

        if (isPlayingJumpscare && !animationComplete)
        {
            State = NodeState.Running;
            return State;
        }

        // Animation finished, end jumpscare
        UnlockPlayer();
        ResetJumpscare();
        State = NodeState.Success;
        return State;
    }

    private void LockPlayer()
    {
        if (player != null && !playerLocked)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                playerLocked = true;
                Debug.Log("Player locked during jumpscare");
            }
            else
            {
                // Try other movement components
                MonoBehaviour[] movementScripts = player.GetComponents<MonoBehaviour>();
                foreach (var script in movementScripts)
                {
                    if (script.GetType().Name.Contains("Move") || script.GetType().Name.Contains("Control"))
                    {
                        script.enabled = false;
                        playerLocked = true;
                        Debug.Log($"Disabled {script.GetType().Name} during jumpscare");
                    }
                }

                if (!playerLocked)
                    Debug.LogWarning("No movement controller found on player to disable");
            }
        }
    }

    private void UnlockPlayer()
    {
        if (player != null && playerLocked)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = true;
                Debug.Log("Player unlocked after jumpscare");
            }
            else
            {
                // Re-enable other movement components
                MonoBehaviour[] movementScripts = player.GetComponents<MonoBehaviour>();
                foreach (var script in movementScripts)
                {
                    if (script.GetType().Name.Contains("Move") || script.GetType().Name.Contains("Control"))
                    {
                        script.enabled = true;
                        Debug.Log($"Re-enabled {script.GetType().Name} after jumpscare");
                    }
                }
            }
            playerLocked = false;
        }
    }

    private void ResetJumpscare()
    {
        started = false;
        rotationElapsed = 0f;
        jumpscareTimer = 0f;
    }
}