using UnityEngine;
using UnityEngine.AI;

public class JumpscareState : StateMachineBehaviour
{
    // ================== CONFIG ==================
    [Header("Focus Target (Holder)")]
    [Tooltip("Điểm focus cho camera trong lúc jumpscare (ưu tiên nếu set).")]
    public Transform holderTransform;

    [Tooltip("Nếu không gán trực tiếp Transform, script sẽ tìm theo tên này trong scene.")]
    public string holderName = "Holder jumpscare camera";

    [Header("Timing (seconds)")]
    [Tooltip("Thời gian mượt khi chuyển VÀO holder.")]
    public float inDuration = 0.35f;

    [Tooltip("Giữ camera ở holder bao lâu.")]
    public float holdDuration = 5.0f;

    [Header("Easing (Into Holder)")]
    public AnimationCurve easeIn = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("NPC Facing")]
    [Tooltip("Quay enemy về phía player khi bắt đầu jumpscare.")]
    public bool facePlayerOnStart = true;

    [Header("Agent Head (Generic Rig)")]
    [Tooltip("Tên bone Head của agent (vd: 'Head', 'mixamorig:Head', 'Bip01 Head').")]
    public string agentHeadBoneName = "Head";


    // ================== INTERNAL REFS ==================
    Transform player;
    NavMeshAgent agent;
    AudioSource audioSource;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HeadBobbingController headBobbingController;

    Camera mainCamera;
    Transform playerCamera;

    // Player lock
    bool playerWasLocked = false;
    CharacterController playerController;
    MonoBehaviour[] playerMovementScripts;

    // Transition state
    enum Phase { ToHolder, Hold, Done }
    Phase phase = Phase.Done;

    // Logic
    Vector3 transitionStartPos;
    Quaternion transitionStartRot;
    float transitionProgress = 0f;

    // Target focus (holder)
    Transform focusPoint;
    float holdTimer = 0f;

    // Animator reference
    Animator cachedAnimator;
    Transform cachedHeadBone;


    // ================== ANIMATOR CALLBACKS ==================
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cachedAnimator = animator;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        audioSource = animator.GetComponent<AudioSource>();
        mainCamera = Camera.main;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (mainCamera == null)
        {
            Debug.LogError("[Jumpscare] Không tìm thấy MainCamera!");
            FailFastExit(animator);
            return;
        }

        playerCamera = mainCamera.transform;

        // Resolve focusPoint (holder)
        focusPoint = holderTransform;
        if (focusPoint == null)
        {
            var found = GameObject.Find(holderName);
            if (found != null) focusPoint = found.transform;
        }
        if (focusPoint == null)
        {
            Debug.LogError($"[Jumpscare] Không tìm thấy Holder '{holderName}' và holderTransform chưa được gán.");
            FailFastExit(animator);
            return;
        }

        // Chuẩn bị phase vào holder
        transitionStartPos = playerCamera.position;
        transitionStartRot = playerCamera.rotation;
        transitionProgress = 0f;
        phase = Phase.ToHolder;
        holdTimer = 0f;

        // Optional: quay enemy về phía player
        if (facePlayerOnStart && player != null)
        {
            Vector3 dir = (player.position - animator.transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                animator.transform.rotation = Quaternion.LookRotation(dir.normalized);
        }

        // Lock & trigger fx
        LockPlayer();
        TriggerJumpscare(animator);

        // Tìm Head bone theo tên
        cachedHeadBone = FindBoneByName(cachedAnimator.transform, agentHeadBoneName);
        if (cachedHeadBone == null)
        {
            Debug.LogWarning($"[Jumpscare] Không tìm thấy bone '{agentHeadBoneName}' trong agent. Sẽ dùng pivot.");
        }

        Debug.Log("[Jumpscare] Begin smooth transition INTO holder.");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerCamera == null) return;

        switch (phase)
        {
            case Phase.ToHolder:
                {
                    Vector3 targetPos = focusPoint.position;

                    // Lấy vị trí Head để nhìn vào
                    Vector3 lookTarget = cachedHeadBone != null ? cachedHeadBone.position : cachedAnimator.transform.position;
                    Vector3 lookDir = (lookTarget - targetPos).normalized;
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);

                    transitionProgress += Time.deltaTime / inDuration;

                    float easedProgress = easeIn.Evaluate(transitionProgress);
                    playerCamera.position = Vector3.Lerp(transitionStartPos, targetPos, easedProgress);
                    playerCamera.rotation = Quaternion.Slerp(transitionStartRot, targetRot, easedProgress);

                    if (transitionProgress >= 1f)
                    {
                        phase = Phase.Hold;
                        holdTimer = 0f;
                        Debug.Log("[Jumpscare] Holding at focus point.");
                    }
                    break;
                }

            case Phase.Hold:
                {
                    holdTimer += Time.deltaTime;
                    if (holdTimer >= holdDuration)
                    {
                        phase = Phase.Done;
                        Debug.Log("[Jumpscare] Done. Waiting for respawn system.");
                    }
                    break;
                }

            case Phase.Done:
            default:
                break;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetAnimatorFlags(animator);

        if (agent != null)
            agent.isStopped = false;

        phase = Phase.Done;
    }


    // ================== HELPERS ==================
    void TriggerJumpscare(Animator animator)
    {
        if (audioSource != null)
        {
            // audioSource.PlayOneShot(jumpscareClip);
        }
    }

    void ResetAnimatorFlags(Animator animator)
    {
        animator.SetBool("IsJumpscaring", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAlert", false);
    }

    void FailFastExit(Animator animator)
    {
        ResetAnimatorFlags(animator);
        if (agent != null) agent.isStopped = false;
    }

    // Tìm bone theo tên trong hierarchy
    Transform FindBoneByName(Transform parent, string boneName)
    {
        if (parent.name == boneName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform found = FindBoneByName(child, boneName);
            if (found != null)
                return found;
        }

        return null;
    }

    // --------- Player Lock / Unlock ----------
    void LockPlayer()
    {
        if (player == null || playerWasLocked) return;

        playerController = player.GetComponent<CharacterController>();
        if (playerController != null)
            playerController.enabled = false;

        playerMovementScripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in playerMovementScripts)
        {
            if (script == null) continue;
            string nm = script.GetType().Name.ToLower();
            if (nm.Contains("move") || nm.Contains("control") || nm.Contains("player") || nm.Contains("first"))
                script.enabled = false;
        }

        playerWasLocked = true;
        Debug.Log("[Jumpscare] Player locked.");
    }
}