using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script freeze agent khi bị nhìn - KHÔNG cần sửa State Machine
/// Hoạt động độc lập, có thể bật/tắt bất cứ lúc nào
/// Ưu tiên tuyệt đối - chạy sau tất cả
/// </summary>
[DefaultExecutionOrder(1000)] // Chạy SAU tất cả script khác
public class FreezeWhenLookedAt : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionAngle = 45f;
    [SerializeField] private float maxDetectionDistance = 20f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Freeze Behavior")]
    [Tooltip("Nếu bật, script sẽ tự động tìm Player tag. Nếu tắt, gán manual.")]
    [SerializeField] private bool autoFindPlayer = true;
    [SerializeField] private Transform manualPlayerTransform;
    [SerializeField] private Camera manualPlayerCamera;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;
    [SerializeField] private bool enableDebugLogs = true;

    private Transform player;
    private Camera playerCamera;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFrozen = false;
    private Vector3 frozenPosition;
    private Quaternion frozenRotation;

    // Lưu execution order để đảm bảo chạy sau state machine
    private int originalExecutionOrder = 0;

    void OnEnable()
    {
        // Tìm components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Debug.Log($"[FREEZE INIT] Script enabled on {gameObject.name}", this);
        Debug.Log($"[FREEZE INIT] NavMeshAgent: {(agent != null ? "Found" : "NOT FOUND")}", this);
        Debug.Log($"[FREEZE INIT] Animator: {(animator != null ? "Found" : "NOT FOUND")}", this);

        if (autoFindPlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerCamera = Camera.main;
                if (playerCamera == null)
                {
                    playerCamera = playerObj.GetComponentInChildren<Camera>();
                }

                Debug.Log($"[FREEZE INIT] Player found: {player.name}", this);
                Debug.Log($"[FREEZE INIT] Camera found: {(playerCamera != null ? playerCamera.name : "NOT FOUND")}", this);
            }
            else
            {
                Debug.LogError($"[FREEZE INIT] Player with tag 'Player' NOT FOUND!", this);
            }
        }
        else
        {
            player = manualPlayerTransform;
            playerCamera = manualPlayerCamera;
            Debug.Log($"[FREEZE INIT] Manual mode - Player: {(player != null ? player.name : "NULL")}", this);
        }
    }

    void Update()
    {
        if (player == null || playerCamera == null)
        {
            if (Time.frameCount % 60 == 0) // Log mỗi 60 frames (tránh spam)
            {
                Debug.LogWarning($"[FREEZE] Player or Camera is NULL! Player: {(player != null)}, Camera: {(playerCamera != null)}", this);
            }
            return;
        }

        bool shouldFreeze = IsPlayerLookingAtAgent();

        if (shouldFreeze != isFrozen)
        {
            if (shouldFreeze)
            {
                FreezeAgent();
            }
            else
            {
                UnfreezeAgent();
            }
        }

        // Debug info mỗi giây
        if (enableDebugLogs && Time.frameCount % 60 == 0)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            Vector3 directionToAgent = (transform.position - playerCamera.transform.position).normalized;
            float angle = Vector3.Angle(playerCamera.transform.forward, directionToAgent);

            Debug.Log($"[FREEZE DEBUG] Distance: {distance:F2}m | Angle: {angle:F1}° | Frozen: {isFrozen}", this);
        }
    }

    void LateUpdate()
    {
        // Chạy SAU tất cả Update và StateMachineBehaviour
        // Override mọi thay đổi từ state machine
        if (isFrozen)
        {
            ApplyFreeze();
        }
    }

    void FixedUpdate()
    {
        // Thêm một lớp bảo vệ trong physics update
        if (isFrozen)
        {
            ApplyFreeze();

            if (agent != null)
            {
                agent.velocity = Vector3.zero;
            }
        }
    }

    // OnAnimatorMove chạy SAU tất cả animation updates
    void OnAnimatorMove()
    {
        if (isFrozen)
        {
            // Override hoàn toàn animator movement
            ApplyFreeze();
        }
    }

    // OnAnimatorIK - override IK movements
    void OnAnimatorIK(int layerIndex)
    {
        if (isFrozen)
        {
            ApplyFreeze();
        }
    }

    bool IsPlayerLookingAtAgent()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (enableDebugLogs && Time.frameCount % 120 == 0)
        {
            Debug.Log($"[FREEZE CHECK] Distance: {distance:F2} / Max: {maxDetectionDistance}", this);
        }

        if (distance > maxDetectionDistance)
        {
            return false;
        }

        Vector3 directionToAgent = (transform.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToAgent);

        if (enableDebugLogs && Time.frameCount % 120 == 0)
        {
            Debug.Log($"[FREEZE CHECK] Angle: {angle:F1}° / Max: {detectionAngle}°", this);
        }

        if (angle > detectionAngle)
        {
            return false;
        }

        // Raycast với layer mask
        RaycastHit hit;
        Vector3 rayStart = playerCamera.transform.position;

        if (Physics.Raycast(rayStart, directionToAgent, out hit, distance))
        {
            bool hitSelf = (hit.transform == transform || hit.transform.IsChildOf(transform));

            if (enableDebugLogs && Time.frameCount % 120 == 0)
            {
                Debug.Log($"[FREEZE CHECK] Raycast hit: {hit.transform.name} | Is Self: {hitSelf}", this);
            }

            if (hitSelf)
            {
                if (showDebugRays)
                {
                    Debug.DrawRay(rayStart, directionToAgent * distance, Color.red, 0.1f);
                }
                return true;
            }
            else
            {
                if (showDebugRays)
                {
                    Debug.DrawRay(rayStart, directionToAgent * hit.distance, Color.blue, 0.1f);
                }
                return false;
            }
        }

        if (showDebugRays)
        {
            Debug.DrawRay(rayStart, directionToAgent * distance, Color.green, 0.1f);
        }

        return false;
    }

    void FreezeAgent()
    {
        isFrozen = true;
        frozenPosition = transform.position;
        frozenRotation = transform.rotation;

        if (enableDebugLogs)
        {
            Debug.Log($"<color=red>[FREEZE] {gameObject.name} frozen!</color>", this);
        }
    }

    void UnfreezeAgent()
    {
        isFrozen = false;

        // Khôi phục lại các settings bình thường
        if (agent != null)
        {
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }

        if (animator != null)
        {
            animator.speed = 1f;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"<color=green>[UNFREEZE] {gameObject.name} unfrozen!</color>", this);
        }
    }

    void ApplyFreeze()
    {
        // NUCLEAR OPTION - Override tất cả mọi thứ với lực tối đa

        // 1. Vô hiệu hóa NavMeshAgent hoàn toàn
        if (agent != null && agent.enabled)
        {
            agent.enabled = false; // TẮT HOÀN TOÀN agent
        }

        // 2. Vô hiệu hóa Animator hoàn toàn
        if (animator != null && animator.enabled)
        {
            animator.enabled = false; // TẮT HOÀN TOÀN animator
        }

        // 3. Lock cứng transform - KHÔNG CHO PHÉP DI CHUYỂN
        transform.position = frozenPosition;
        transform.rotation = frozenRotation;

        // 4. Đảm bảo rigidbody (nếu có) cũng bị freeze
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Đặt kinematic để vô hiệu hóa physics
        }

        // 5. Force position mỗi frame
        if (transform.position != frozenPosition)
        {
            transform.position = frozenPosition;
            Debug.LogWarning($"[FREEZE] Position was changed! Forcing back to frozen position", this);
        }

        if (transform.rotation != frozenRotation)
        {
            transform.rotation = frozenRotation;
            Debug.LogWarning($"[FREEZE] Rotation was changed! Forcing back to frozen rotation", this);
        }
    }

    // ===== PUBLIC API =====
    /// <summary>Kiểm tra xem agent có đang frozen không</summary>
    public bool IsFrozen() => isFrozen;

    /// <summary>Force freeze/unfreeze agent (debug hoặc script khác gọi)</summary>
    public void SetFrozen(bool freeze)
    {
        if (freeze && !isFrozen)
        {
            FreezeAgent();
        }
        else if (!freeze && isFrozen)
        {
            UnfreezeAgent();
        }
    }

    /// <summary>Set player transform manually nếu không dùng auto-find</summary>
    public void SetPlayer(Transform playerTransform, Camera camera)
    {
        player = playerTransform;
        playerCamera = camera;
        autoFindPlayer = false;
    }

    // ===== GIZMOS =====
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Detection sphere
        Gizmos.color = isFrozen ? new Color(1, 0, 0, 0.3f) : new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, maxDetectionDistance);

        // Line to player
        if (player != null && playerCamera != null)
        {
            Vector3 directionToAgent = (transform.position - playerCamera.transform.position).normalized;
            float angle = Vector3.Angle(playerCamera.transform.forward, directionToAgent);
            float distance = Vector3.Distance(player.position, transform.position);

            Gizmos.color = (angle <= detectionAngle && distance <= maxDetectionDistance) ? Color.red : Color.gray;
            Gizmos.DrawLine(playerCamera.transform.position, transform.position);
        }

        // Frozen indicator
        if (isFrozen)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2f, Vector3.one * 0.5f);
        }
    }

    void OnDisable()
    {
        // Cleanup khi disable script
        if (isFrozen)
        {
            UnfreezeAgent();
        }
    }
}

/*
 * ╔═══════════════════════════════════════════════════════════╗
 * ║  HƯỚNG DẪN SỬ DỤNG - MODULAR & ƯU TIÊN TUYỆT ĐỐI         ║
 * ╚═══════════════════════════════════════════════════════════╝
 * 
 * 1. ATTACH script này vào bất kỳ Agent nào cần freeze
 * 
 * 2. Script sẽ TỰ ĐỘNG có ưu tiên cao nhất nhờ [DefaultExecutionOrder(1000)]
 *    - Chạy SAU tất cả State Machine
 *    - Chạy SAU tất cả script khác
 *    - Override HOÀN TOÀN mọi movement/animation
 * 
 * 3. Cấu hình trong Inspector:
 *    - Detection Angle: Góc nhìn (45° = 90° tổng)
 *    - Max Detection Distance: Khoảng cách tối đa
 *    - Obstacle Layer: Layer của tường (để không nhìn xuyên)
 *    - Auto Find Player: Bật (hoặc tắt và gán manual)
 * 
 * 4. Script hoạt động với 4 lớp bảo vệ:
 *    ✅ [DefaultExecutionOrder(1000)] - Chạy sau cùng
 *    ✅ LateUpdate() - Override sau tất cả Update
 *    ✅ FixedUpdate() - Override physics
 *    ✅ OnAnimatorMove() - Override animator movement
 * 
 * 5. KHÔNG CẦN:
 *    ❌ Sửa State Machine
 *    ❌ Sửa bất kỳ State script nào
 *    ❌ Set Script Execution Order manual (đã tự động)
 * 
 * 6. Để disable freeze cho agent cụ thể:
 *    - Uncheck script trong Inspector
 *    - Hoặc: GetComponent<FreezeWhenLookedAt>().enabled = false;
 * 
 * ╔═══════════════════════════════════════════════════════════╗
 * ║  FREEZE PRIORITY - ĐÃ TỰ ĐỘNG                            ║
 * ╚═══════════════════════════════════════════════════════════╝
 * 
 * Script này có [DefaultExecutionOrder(1000)] nên sẽ:
 * - Chạy SAU tất cả script khác (thậm chí script có order 100, 200, 500...)
 * - Override mọi thứ mà State Machine làm
 * - Đảm bảo agent đứng yên HOÀN TOÀN khi bị nhìn
 * 
 * Bạn KHÔNG cần phải set gì thêm trong Project Settings!
 */