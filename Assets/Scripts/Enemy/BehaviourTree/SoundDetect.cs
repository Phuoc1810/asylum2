using UnityEngine;
using UnityEngine.AI;

public class SoundDetection : MonoBehaviour
{
    public float hearingRadius = 20f;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Filter Settings")]
    [Tooltip("Chỉ detect AudioSource có tag này (để trống = detect tất cả)")]
    public string detectableTag = "AudioDetect";

    [Header("Priority Settings")]
    [Tooltip("Tên parameter trong Animator để check xem có đang chase player không")]
    public string chasingParameterName = "IsRunning";
    public string alertParameterName = "IsAlert";

    [Header("Debug")]
    public bool showDebug = true;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (showDebug)
            Debug.Log($"SoundDetection started. Looking for tag: '{detectableTag}', Hearing radius: {hearingRadius}m");
    }

    void Update()
    {
        DetectSound();
    }

    void DetectSound()
    {
        // PRIORITY CHECK: Nếu đang chase/alert player thì KHÔNG detect sound
        if (animator != null)
        {
            bool isChasing = animator.GetBool(chasingParameterName);
            bool isAlert = animator.GetBool(alertParameterName);

            if (isChasing || isAlert)
            {
                if (showDebug)
                    Debug.Log("<color=yellow>Agent is chasing/alert player - Sound detection DISABLED</color>");
                return;
            }
        }

        // Tìm tất cả AudioSource đang phát trong bán kính
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        if (showDebug)
            Debug.Log($"Found {allAudioSources.Length} total AudioSources in scene");

        AudioSource nearestSound = null;
        float nearestDistance = Mathf.Infinity;
        int validSources = 0;

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (showDebug)
                Debug.Log($"Checking AudioSource on '{audioSource.gameObject.name}' - Tag: '{audioSource.tag}', IsPlaying: {audioSource.isPlaying}");

            // Bỏ qua nếu không đúng tag
            if (!string.IsNullOrEmpty(detectableTag) && !audioSource.CompareTag(detectableTag))
            {
                if (showDebug)
                    Debug.Log($"  -> Skipped: Wrong tag (expected '{detectableTag}', got '{audioSource.tag}')");
                continue;
            }

            // Chỉ phát hiện AudioSource đang phát
            if (audioSource.isPlaying)
            {
                float distance = Vector3.Distance(transform.position, audioSource.transform.position);

                if (showDebug)
                    Debug.Log($"  -> Valid source! Distance: {distance:F2}m (hearing radius: {hearingRadius}m)");

                // Kiểm tra trong bán kính nghe
                if (distance <= hearingRadius && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSound = audioSource;
                    validSources++;

                    if (showDebug)
                        Debug.Log($"  -> NEW NEAREST SOUND at {distance:F2}m");
                }
            }
            else
            {
                if (showDebug)
                    Debug.Log($"  -> Skipped: Not playing");
            }
        }

        // Nếu tìm thấy âm thanh, di chuyển đến đó
        if (nearestSound != null)
        {
            agent.SetDestination(nearestSound.transform.position);

            if (showDebug)
                Debug.Log($"<color=green>MOVING TO SOUND at {nearestSound.gameObject.name}, position: {nearestSound.transform.position}, distance: {nearestDistance:F2}m</color>");
        }
        else
        {
            if (showDebug && allAudioSources.Length > 0)
                Debug.Log("<color=red>No valid sound found in hearing range</color>");
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebug) return;

        // Vẽ bán kính nghe
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}