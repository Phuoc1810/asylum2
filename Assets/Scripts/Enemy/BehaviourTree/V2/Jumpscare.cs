using UnityEngine;
using UnityEngine.AI;

public class CloseToPlayer : Node
{
    public CloseToPlayer(Transform ai, Transform player, float closeRange = 5f, string name = "CloseToPlayer", int priority = 0)
        : base(name, priority)
    {
        IStrategy strategy = new ConditionStrategy(() =>
        {
            if (ai == null || player == null)
            {
                Debug.LogWarning("CloseToPlayer: AI or Player is null");
                return false;
            }
            float dist = Vector3.Distance(ai.position, player.position);
            Debug.Log($"CloseToPlayer: Distance to player = {dist:F2}, CloseRange = {closeRange}");
            return dist <= closeRange;
        });
        AddChild(new Leaf(name, strategy, priority));
    }

    public override NodeStatus Process()
    {
        return base.Process();
    }
}

public class JumpscareStrategy : IStrategy
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
    private float jumpscareTimeout = 5f;
    private float jumpscareTimer = 0f;
    private NavMeshAgent agent;

    public JumpscareStrategy(Transform player, Transform enemy, Animator animator, Transform playerCamera,
        string jumpscareTrigger = "Jumpscare", float rotationTime = 0.3f)
    {
        this.player = player;
        this.enemy = enemy;
        this.animator = animator;
        this.jumpscareTrigger = jumpscareTrigger;
        this.playerCamera = playerCamera;
        this.rotationTime = rotationTime;
        this.agent = enemy.GetComponent<NavMeshAgent>();
    }

    public NodeStatus Process()
    {
        // Safety checks
        if (player == null || enemy == null || animator == null || playerCamera == null || agent == null)
        {
            //Debug.LogError($"JumpscareStrategy failed: player={player}, enemy={enemy}, animator={animator}, playerCamera={playerCamera}, agent={agent}");
            return NodeStatus.Failure;
        }

        if (!started)
        {
            // Start jumpscare
            int triggerHash = Animator.StringToHash(jumpscareTrigger);
            if (animator.HasState(0, triggerHash))
            {
                animator.Play(triggerHash);
                Debug.Log($"Playing jumpscare animation: {jumpscareTrigger}");
            }
            else
            {
                Debug.LogError($"Animation state '{jumpscareTrigger}' not found in animator!");
                return NodeStatus.Failure;
            }

            // Lock player movement and stop agent
            LockPlayer();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            started = true;
            rotationElapsed = 0f;
            jumpscareTimer = 0f;
            Debug.Log("Jumpscare started");
            return NodeStatus.Running;
        }

        // Update timers
        jumpscareTimer += Time.deltaTime;

        // Timeout safety
        if (jumpscareTimer > jumpscareTimeout)
        {
            Debug.LogWarning("Jumpscare timeout reached, ending jumpscare");
            UnlockPlayer();
            Reset();
            return NodeStatus.Success;
        }

        // Handle camera rotation during jumpscare
        if (rotationElapsed < rotationTime)
        {
            rotationElapsed += Time.deltaTime;
            Vector3 lookDir = (enemy.position - playerCamera.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            float t = Mathf.Clamp01(rotationElapsed / rotationTime);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, t);
            return NodeStatus.Running;
        }

        // Check if animation is still playing
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingJumpscare = stateInfo.IsName(jumpscareTrigger);
        bool animationComplete = stateInfo.normalizedTime >= 1.0f;

        if (isPlayingJumpscare && !animationComplete)
        {
            return NodeStatus.Running;
        }

        // Animation finished, end jumpscare
        UnlockPlayer();
        Reset();
        Debug.Log("Jumpscare completed successfully");
        return NodeStatus.Success;
    }

    public void Reset()
    {
        started = false;
        rotationElapsed = 0f;
        jumpscareTimer = 0f;
        if (agent != null)
        {
            agent.isStopped = false;
        }
    }

    private void LockPlayer()
    {
        if (player != null && !playerLocked)
        {
            // Try CharacterController
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                playerLocked = true;
                Debug.Log("Player locked during jumpscare (CharacterController)");
                return;
            }

            // Try common movement scripts
            MonoBehaviour[] movementScripts = player.GetComponents<MonoBehaviour>();
            bool foundMovement = false;
            foreach (var script in movementScripts)
            {
                string scriptName = script.GetType().Name.ToLower();
                if (scriptName.Contains("move") || scriptName.Contains("control") || scriptName.Contains("player"))
                {
                    script.enabled = false;
                    foundMovement = true;
                    playerLocked = true;
                    Debug.Log($"Disabled {script.GetType().Name} during jumpscare");
                }
            }
            if (!foundMovement)
                Debug.LogWarning("No movement controller found on player to disable");
        }
    }

    private void UnlockPlayer()
    {
        if (player != null && playerLocked)
        {
            // Try CharacterController
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = true;
                Debug.Log("Player unlocked after jumpscare (CharacterController)");
            }

            // Try common movement scripts
            MonoBehaviour[] movementScripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in movementScripts)
            {
                string scriptName = script.GetType().Name.ToLower();
                if (scriptName.Contains("move") || scriptName.Contains("control") || scriptName.Contains("player"))
                {
                    script.enabled = true;
                    Debug.Log($"Re-enabled {script.GetType().Name} after jumpscare");
                }
            }
            playerLocked = false;
        }
    }
}

public class Jumpscare : Node
{
    public Jumpscare(Transform player, Transform enemy, Animator animator, Transform playerCamera,
        string jumpscareTrigger = "Jumpscare", float rotationTime = 0.3f, string name = "Jumpscare", int priority = 0)
        : base(name, priority)
    {
        IStrategy strategy = new JumpscareStrategy(player, enemy, animator, playerCamera, jumpscareTrigger, rotationTime);
        AddChild(new Leaf(name, strategy, priority));
    }

    public override NodeStatus Process()
    {
        return base.Process();
    }
}