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
    public float holdDuration = 5.0f;  // tăng mặc định để bạn thấy chậm hơn

    [Tooltip("KHÔNG dùng nếu đã bật SmoothDamp cho phase quay về (bên dưới).")]
    public float outDuration = 0.4f;

    [Header("Easing (Into Holder)")]
    public AnimationCurve easeIn = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("NPC Facing")]
    [Tooltip("Quay enemy về phía player khi bắt đầu jumpscare.")]
    public bool facePlayerOnStart = true;

    [Header("Return Smoothing (From Holder -> Original)")]
    [Tooltip("Dùng SmoothDamp để quay về. Bật để có cảm giác mượt và kiểm soát tốc độ tốt hơn.")]
    public bool useSmoothReturn = true;

    [Tooltip("Thời gian đáp ứng (time constant) cho pha quay về gốc. Lớn hơn = chậm hơn.")]
    public float returnSmoothTime = 0.9f;    // chỉnh 0.6–1.2 tùy ý

    [Tooltip("Giới hạn tốc độ tối đa khi quay về (m/s). 0 = không giới hạn.")]
    public float maxReturnSpeed = 0f;        // 0 = off

    [Tooltip("Ngưỡng coi như đã về tới điểm gốc (m).")]
    public float returnDistanceEpsilon = 0.01f;

    [Tooltip("Ngưỡng coi như đã khớp góc (độ).")]
    public float returnAngleEpsilon = 0.5f;


    // ================== INTERNAL REFS ==================
    Transform player;
    NavMeshAgent agent;
    AudioSource audioSource;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HeadBobbingController headBobbingController;

    Camera mainCamera;
    Transform playerCamera;                 // = mainCamera.transform
    Vector3 originalCameraPosition;
    Quaternion originalCameraRotation;
    Transform originalCameraParent;

    // Player lock
    bool playerWasLocked = false;
    CharacterController playerController;
    MonoBehaviour[] playerMovementScripts;

    // Transition state
    enum Phase { ToHolder, Hold, ToOriginal, Done }
    Phase phase = Phase.Done;

    // Logic theo ý bạn
    Vector3 transitionStartPos;
    Quaternion transitionStartRot;
    float transitionProgress = 0f;

    // Target focus (holder)
    Transform focusPoint;
    float holdTimer = 0f;

    // Velocity cho SmoothDamp
    Vector3 _posVelocity;
    Vector3 _rotVelocity; // x=pitch, y=yaw, z=roll


    // ================== ANIMATOR CALLBACKS ==================
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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

        // Lưu trạng thái camera gốc
        originalCameraPosition = playerCamera.position;
        originalCameraRotation = playerCamera.rotation;
        originalCameraParent = playerCamera.parent; // dự phòng nếu bạn muốn re-parent sau này

        // Chuẩn bị phase vào holder (LERP/SLERP)
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
                    Quaternion targetRot = focusPoint.rotation;

                    transitionProgress += Time.deltaTime * 1;

                    playerCamera.position = Vector3.Lerp(transitionStartPos, targetPos, transitionProgress);
                    playerCamera.rotation = Quaternion.Slerp(transitionStartRot, targetRot, transitionProgress);

                    if (transitionProgress >= 1)
                    {
                        CompleteTransition();
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
        // Đảm bảo restore nếu thoát sớm
        
        //UnlockPlayer();
        ResetAnimatorFlags(animator);

        if (agent != null)
            agent.isStopped = false;

        phase = Phase.Done;
    }


    // ================== HELPERS ==================
    void TriggerJumpscare(Animator animator)
    {
        // Ví dụ phát âm thanh:
        if (audioSource != null)
        {
            // audioSource.PlayOneShot(jumpscareClip);
        }
        // Có thể add CameraShake ở đây nếu bạn có hệ thống rung camera
        // CameraShake.Instance.ShakeCamera(1f, 0.5f);
    }

    void ResetAnimatorFlags(Animator animator)
    {
        animator.SetBool("IsJumpscaring", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAlert", false);
    }

    void FailFastExit(Animator animator)
    {
        //UnlockPlayer();
        ResetAnimatorFlags(animator);
        if (agent != null) agent.isStopped = false;
    }

    // Hook theo API bạn đưa; gọi khi một pha chuyển kết thúc
    void CompleteTransition()
    {
        originalCameraPosition = focusPoint.position;
        originalCameraRotation = focusPoint.rotation;

        phase = Phase.Done;
        transitionProgress = 0f;
    }

    void EndAndRestore(Animator animator)
    {
        CompleteTransition();
        phase = Phase.Done;

        //UnlockPlayer();
        ResetAnimatorFlags(animator);
        if (agent != null) agent.isStopped = false;

        Debug.Log("[Jumpscare] Done. Camera restored to original.");
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

    void UnlockPlayer()
    {
        if (!playerWasLocked || player == null) return;

        if (playerController != null)
            playerController.enabled = true;

        if (playerMovementScripts != null)
        {
            foreach (var script in playerMovementScripts)
            {
                if (script == null) continue;
                string nm = script.GetType().Name.ToLower();
                if (nm.Contains("move") || nm.Contains("control") || nm.Contains("player") || nm.Contains("first"))
                    script.enabled = true;
            }
        }

        playerWasLocked = false;
        Debug.Log("[Jumpscare] Player unlocked.");
    }
}
