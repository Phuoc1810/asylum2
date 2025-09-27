using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class JumpscareState : StateMachineBehaviour
{
    Transform player;
    NavMeshAgent agent;
    bool jumpscareStarted = false;
    float jumpscareTimer = 0f;
    float jumpscareDuration = 3f;

    // Player locking variables
    bool playerWasLocked = false;
    CharacterController playerController;
    MonoBehaviour[] playerMovementScripts;

    // References để gọi jumpscare effects
    GameObject jumpscareUI;
    AudioSource audioSource;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        jumpscareStarted = false;
        jumpscareTimer = 0f;
        playerWasLocked = false;

        // Tìm UI và audio references
        jumpscareUI = GameObject.FindGameObjectWithTag("JumpscareUI");
        audioSource = animator.GetComponent<AudioSource>();

        // Lock player movement
        LockPlayer();

        // Trigger jumpscare effects
        TriggerJumpscare();

        Debug.Log("JUMPSCARE! Player caught!");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        if (!jumpscareStarted)
        {
            // Face player instantly
            Vector3 direction = (player.position - animator.transform.position).normalized;
            animator.transform.rotation = Quaternion.LookRotation(direction);
            jumpscareStarted = true;
        }

        jumpscareTimer += Time.deltaTime;

        // End jumpscare after duration
        if (jumpscareTimer >= jumpscareDuration)
        {
            // Unlock player
            UnlockPlayer();

            // Hide jumpscare UI
            if (jumpscareUI != null)
            {
                jumpscareUI.SetActive(false);
            }

            // Reset tất cả các parameters về trạng thái ban đầu
            animator.SetBool("IsJumpscaring", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAlert", false);

            // Có thể teleport quái vật về vị trí ban đầu
            TeleportAwayFromPlayer(animator);

            Debug.Log("Jumpscare completed, returning to idle");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UnlockPlayer();

        // Hide jumpscare UI
        if (jumpscareUI != null)
        {
            jumpscareUI.SetActive(false);
        }

        if (agent != null)
        {
            agent.isStopped = false;
        }
    }

    void TriggerJumpscare()
    {
        // Hiển thị UI jumpscare
        if (jumpscareUI != null)
        {
            jumpscareUI.SetActive(true);
        }

        // Phát âm thanh jumpscare
        if (audioSource != null)
        {
            // audioSource.PlayOneShot(jumpscareSound);
        }

        // Có thể thêm camera shake
        // CameraShake.Instance.ShakeCamera(1f, 0.5f);
    }

    void TeleportAwayFromPlayer(Animator animator)
    {
        // Tìm vị trí xa người chơi để teleport
        Vector3 randomDirection = Random.insideUnitSphere * 20f;
        randomDirection += animator.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 20f, NavMesh.AllAreas))
        {
            animator.transform.position = hit.position;
        }
    }

    void LockPlayer()
    {
        if (player == null || playerWasLocked) return;

        // Try CharacterController first
        playerController = player.GetComponent<CharacterController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Disable common movement scripts
        playerMovementScripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in playerMovementScripts)
        {
            string scriptName = script.GetType().Name.ToLower();
            if (scriptName.Contains("move") || scriptName.Contains("control") ||
                scriptName.Contains("player") || scriptName.Contains("first"))
            {
                script.enabled = false;
            }
        }

        playerWasLocked = true;
        Debug.Log("Player movement locked during jumpscare");
    }

    void UnlockPlayer()
    {
        if (player == null || !playerWasLocked) return;

        // Re-enable CharacterController
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Re-enable movement scripts
        if (playerMovementScripts != null)
        {
            foreach (var script in playerMovementScripts)
            {
                string scriptName = script.GetType().Name.ToLower();
                if (scriptName.Contains("move") || scriptName.Contains("control") ||
                    scriptName.Contains("player") || scriptName.Contains("first"))
                {
                    script.enabled = true;
                }
            }
        }

        playerWasLocked = false;
        Debug.Log("Player movement unlocked");
    }
}