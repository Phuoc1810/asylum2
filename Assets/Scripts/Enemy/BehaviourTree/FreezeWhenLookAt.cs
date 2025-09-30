using UnityEngine;
using UnityEngine.AI;

public class FreezeWhenLookedAt : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionAngle = 45f;
    [SerializeField] private float maxDetectionDistance = 20f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

    private Transform player;
    private Camera playerCamera;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFrozen = false;
    private Vector3 frozenPosition;
    private Quaternion frozenRotation;

    void Start()
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
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null || playerCamera == null) return;

        bool shouldFreeze = IsPlayerLookingAtAgent();

        if (shouldFreeze && !isFrozen)
        {
            FreezeAgent();
        }
        else if (!shouldFreeze && isFrozen)
        {
            UnfreezeAgent();
        }
    }

    void LateUpdate()
    {
        // FORCE freeze trong LateUpdate để override mọi thay đổi từ state machine
        if (isFrozen)
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                agent.ResetPath(); // Xóa path hiện tại
            }

            if (animator != null)
            {
                animator.speed = 0f;
            }

            // Lock vị trí và rotation
            transform.position = frozenPosition;
            transform.rotation = frozenRotation;
        }
    }

    bool IsPlayerLookingAtAgent()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > maxDetectionDistance)
        {
            return false;
        }

        Vector3 directionToAgent = (transform.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToAgent);

        if (angle > detectionAngle)
        {
            return false;
        }

        // Raycast kiểm tra vật cản
        RaycastHit hit;
        Vector3 rayStart = playerCamera.transform.position;

        if (Physics.Raycast(rayStart, directionToAgent, out hit, distance))
        {
            // Kiểm tra xem raycast có chạm vào agent không
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (showDebugRays)
                {
                    Debug.DrawRay(rayStart, directionToAgent * distance, Color.red);
                }
                return true;
            }
            else
            {
                // Chạm vào vật cản khác
                if (showDebugRays)
                {
                    Debug.DrawRay(rayStart, directionToAgent * hit.distance, Color.blue);
                }
                return false;
            }
        }

        if (showDebugRays)
        {
            Debug.DrawRay(rayStart, directionToAgent * distance, Color.green);
        }

        return false;
    }

    void FreezeAgent()
    {
        isFrozen = true;

        // Lưu vị trí và rotation hiện tại
        frozenPosition = transform.position;
        frozenRotation = transform.rotation;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }

        if (animator != null)
        {
            animator.speed = 0f;
        }

        Debug.Log($"<color=red>Agent FROZEN at {Time.time}</color>");
    }

    void UnfreezeAgent()
    {
        isFrozen = false;

        if (agent != null)
        {
            agent.isStopped = false;
        }

        if (animator != null)
        {
            animator.speed = 1f;
        }

        Debug.Log($"<color=green>Agent UNFROZEN at {Time.time}</color>");
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    public void ForceFreeze(bool freeze)
    {
        if (freeze)
        {
            FreezeAgent();
        }
        else
        {
            UnfreezeAgent();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && playerCamera != null)
        {
            // Vẽ detection range
            Gizmos.color = isFrozen ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxDetectionDistance);

            // Vẽ line đến player
            if (player != null)
            {
                Vector3 directionToAgent = (transform.position - playerCamera.transform.position).normalized;
                float angle = Vector3.Angle(playerCamera.transform.forward, directionToAgent);

                Gizmos.color = angle <= detectionAngle ? Color.red : Color.green;
                Gizmos.DrawLine(playerCamera.transform.position, transform.position);
            }
        }
    }
}