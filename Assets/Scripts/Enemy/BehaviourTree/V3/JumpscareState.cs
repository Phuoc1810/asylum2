using UnityEngine;
using UnityEngine.AI;

public class JumpscareState : StateMachineBehaviour
{
    // ================== CONFIG ==================
    [Header("Focus Target")]
    [Tooltip("Điểm focus cho camera trong lúc jumpscare.")]
    public Transform holderTransform;

    [Tooltip("Tên holder trong scene. Có thể dùng {AgentName} để tự động thay thế.")]
    public string holderName = "Holder jumpscare camera";

    [Tooltip("Tìm holder là con của Agent thay vì tìm trong toàn scene.")]
    public bool holderIsChildOfAgent = false;

    [Header("Timing")]
    public float inDuration = 0.35f;
    public float holdDuration = 5.0f;

    [Header("Camera Easing")]
    public AnimationCurve easeIn = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Agent Head Target")]
    [Tooltip("Tên bone Head (để trống nếu không có bone).")]
    public string agentHeadBoneName = "Head";

    [Tooltip("Offset từ root đến mặt (dùng khi không có bone).")]
    public Vector3 headOffset = new Vector3(0, 1.7f, 0.3f);

    [Tooltip("Bỏ qua bone, chỉ dùng offset.")]
    public bool useOffsetOnly = false;

    [Header("Options")]
    public bool facePlayerOnStart = true;

    // ================== INTERNAL ==================
    Transform player;
    NavMeshAgent agent;
    Camera mainCamera;
    Transform playerCamera;

    enum Phase { ToHolder, Hold, Done }
    Phase phase = Phase.Done;

    Vector3 startPos;
    Quaternion startRot;
    float progress = 0f;
    float holdTimer = 0f;

    Transform focusPoint;
    Animator cachedAnimator;
    Transform cachedHeadBone;

    bool playerLocked = false;
    CharacterController playerController;
    MonoBehaviour[] playerScripts;

    // ================== CALLBACKS ==================
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cachedAnimator = animator;
        mainCamera = Camera.main;
        playerCamera = mainCamera?.transform;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();

        if (mainCamera == null || playerCamera == null)
        {
            Debug.LogError("[Jumpscare] Không tìm thấy MainCamera!");
            ExitState(animator);
            return;
        }

        // Stop agent
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // Find holder
        focusPoint = holderTransform;
        if (focusPoint == null)
        {
            Debug.Log($"[Jumpscare] Đang tìm holder theo tên: '{holderName}'");
            GameObject holderObj = GameObject.Find(holderName);

            if (holderObj == null)
            {
                // Debug: List tất cả objects có tên gần giống
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                Debug.LogWarning($"[Jumpscare] Tìm thấy {allObjects.Length} objects trong scene:");
                foreach (var obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("holder") || obj.name.ToLower().Contains("camera"))
                    {
                        Debug.Log($"  - '{obj.name}' (active: {obj.activeInHierarchy})");
                    }
                }

                Debug.LogError($"[Jumpscare] Không tìm thấy holder '{holderName}'!");
                ExitState(animator);
                return;
            }

            focusPoint = holderObj.transform;
            Debug.Log($"[Jumpscare] Tìm thấy holder: {holderObj.name}");
        }

        // Setup transition
        startPos = playerCamera.position;
        startRot = playerCamera.rotation;
        progress = 0f;
        holdTimer = 0f;
        phase = Phase.ToHolder;

        // Face player
        if (facePlayerOnStart && player != null)
        {
            Vector3 dir = player.position - animator.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                animator.transform.rotation = Quaternion.LookRotation(dir.normalized);
        }

        // Find head bone
        cachedHeadBone = useOffsetOnly ? null : FindBone(cachedAnimator.transform, agentHeadBoneName);

        LockPlayer();

        Debug.Log("[Jumpscare] Started");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerCamera == null) return;

        if (phase == Phase.ToHolder)
        {
            progress += Time.deltaTime / inDuration;
            float t = easeIn.Evaluate(progress);

            Vector3 headPos = cachedHeadBone != null
                ? cachedHeadBone.position
                : cachedAnimator.transform.TransformPoint(headOffset);

            Vector3 targetPos = focusPoint.position;
            Quaternion targetRot = Quaternion.LookRotation((headPos - targetPos).normalized);

            playerCamera.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, t);

            if (progress >= 1f)
            {
                phase = Phase.Hold;
                holdTimer = 0f;
            }
        }
        else if (phase == Phase.Hold)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdDuration)
            {
                phase = Phase.Done;
                Debug.Log("[Jumpscare] Done");
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExitState(animator);
    }

    // ================== HELPERS ==================
    void ExitState(Animator animator)
    {
        animator.SetBool("IsJumpscaring", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAlert", false);

        if (agent != null)
            agent.isStopped = false;

        phase = Phase.Done;
    }

    Transform FindBone(Transform parent, string boneName)
    {
        if (string.IsNullOrEmpty(boneName) || parent == null)
            return null;

        if (parent.name == boneName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform found = FindBone(child, boneName);
            if (found != null)
                return found;
        }

        return null;
    }

    void LockPlayer()
    {
        if (player == null || playerLocked) return;

        playerController = player.GetComponent<CharacterController>();
        if (playerController != null)
            playerController.enabled = false;

        playerScripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in playerScripts)
        {
            if (script == null) continue;
            string name = script.GetType().Name.ToLower();
            if (name.Contains("move") || name.Contains("control") || name.Contains("player"))
                script.enabled = false;
        }

        playerLocked = true;
    }
}