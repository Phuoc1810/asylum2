using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script xử lý cơ chế Blink (freeze khi bị nhìn)
/// Có thể disable khi jumpscare để enemy tiếp tục di chuyển
/// </summary>
public class BlinkMechanic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Detection Settings")]
    [SerializeField] private float fieldOfViewAngle = 60f;
    [SerializeField] private float visionRange = 30f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;
    [SerializeField] private bool enableBlinkMechanic = true;

    // Cached values
    private bool isFrozen = false;
    private float cosFieldOfView;

    void Awake()
    {
        // Auto-find references
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (!playerCamera)
            playerCamera = Camera.main ?? player?.GetComponentInChildren<Camera>();

        if (!agent)
            agent = GetComponent<NavMeshAgent>();

        if (!animator)
            animator = GetComponent<Animator>();

        cosFieldOfView = Mathf.Cos(fieldOfViewAngle * Mathf.Deg2Rad);
    }

    void Update()
    {
        if (!enableBlinkMechanic || !player) return;

        bool isBeingSeen = IsPlayerLookingAtEnemy();
        isFrozen = isBeingSeen;

        if (showDebugRays)
            DrawDebugRay(isBeingSeen);
    }

    // LateUpdate để FORCE freeze sau khi state machine đã update
    void LateUpdate()
    {
        if (!agent) return;

        // Khi blink bị tắt, FORCE animation chạy
        if (!enableBlinkMechanic)
        {
            if (animator)
                animator.speed = 1f;
            return;
        }

        if (isFrozen)
        {
            // Force freeze
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();

            if (animator)
                animator.speed = 0f;
        }
        else
        {
            // Không unfreeze tự động - để state machine tự quản lý
            // Chỉ restore animator
            if (animator)
                animator.speed = 1f;
        }
    }

    bool IsPlayerLookingAtEnemy()
    {
        if (!player || !playerCamera) return false;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > visionRange) return false;

        // Frustum check
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(transform.position);
        if (viewportPoint.z <= 0 || viewportPoint.x <= 0 || viewportPoint.x >= 1 ||
            viewportPoint.y <= 0 || viewportPoint.y >= 1)
            return false;

        // Dot product check
        Vector3 directionToEnemy = (transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, directionToEnemy);

        if (dot < cosFieldOfView) return false;

        // Raycast
        if (Physics.Raycast(playerCamera.transform.position, directionToEnemy, out RaycastHit hit, distance))
        {
            return hit.transform == transform || hit.transform.IsChildOf(transform);
        }

        return false;
    }

    void DrawDebugRay(bool isBeingSeen)
    {
        if (!playerCamera) return;

        Debug.DrawLine(playerCamera.transform.position, transform.position,
            isBeingSeen ? Color.red : Color.green);

        if (isFrozen)
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 3f, Color.cyan);
    }

    // ========== PUBLIC API ==========

    /// <summary>
    /// Bắt đầu jumpscare - tắt blink mechanic
    /// </summary>
    public void StartJumpscare()
    {
        enableBlinkMechanic = false;
        isFrozen = false;

        // Unfreeze ngay lập tức
        if (agent) agent.isStopped = false;
        if (animator) animator.speed = 1f;

        Debug.Log("[BlinkMechanic] Jumpscare started - Blink disabled");
    }

    /// <summary>
    /// Kết thúc jumpscare - bật lại blink mechanic
    /// </summary>
    public void EndJumpscare()
    {
        enableBlinkMechanic = true;
        Debug.Log("[BlinkMechanic] Jumpscare ended - Blink enabled");
    }

    /// <summary>
    /// Check xem có đang jumpscare không (blink bị tắt)
    /// </summary>
    public bool IsJumpscaring() => !enableBlinkMechanic;

    public bool IsFrozen() => isFrozen && enableBlinkMechanic;
    public void SetEnabled(bool enabled) => enableBlinkMechanic = enabled;
}