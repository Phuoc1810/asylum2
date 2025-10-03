using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script đơn giản chỉ xử lý cơ chế Blink (freeze khi bị nhìn)
/// Có thể dùng cùng với bất kỳ state machine nào
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
        if (!enableBlinkMechanic || !agent) return;

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

    // Public API
    public bool IsFrozen() => isFrozen;
    public void SetEnabled(bool enabled) => enableBlinkMechanic = enabled;
}