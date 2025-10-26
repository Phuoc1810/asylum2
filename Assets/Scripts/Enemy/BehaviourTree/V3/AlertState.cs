using UnityEngine;
using UnityEngine.AI;

public class AlertState : StateMachineBehaviour
{
    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer; // Gán layer "Wall" trong Inspector

    [Header("Audio Settings")]
    [SerializeField] private AudioClip alertSound; // Âm thanh khi phát hiện player
    [SerializeField] private float alertVolume = 1f; // Âm lượng alert sound
    [SerializeField] private bool loopAlertSound = true; // Lặp lại âm thanh alert
    [SerializeField] private float timeBetweenAlertSounds = 1f; // Thời gian giữa các lần phát (nếu loop)

    float timer;
    Transform player;
    NavMeshAgent agent;
    float chaseRange = 20f;
    float losePlayerRange = 20f;
    float alertDuration = 3f;

    // Audio variables
    private AudioSource audioSource;
    private float nextAlertTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 1f;
        agent.SetDestination(agent.transform.position);
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ✅ SETUP AUDIO
        SetupAudio(animator);

        // ✅ PHÁT ÂM THANH ALERT NGAY LẬP TỨC
        if (alertSound != null)
        {
            PlayAlertSound();

            // Lên lịch lần phát tiếp theo nếu loop
            if (loopAlertSound)
            {
                nextAlertTime = Time.time + timeBetweenAlertSounds;
            }
        }

        //Debug.Log("Alert State Started - Enemy detected player!");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ✅ PHÁT LẠI ALERT SOUND NẾU BẬT LOOP
        if (loopAlertSound && alertSound != null && Time.time >= nextAlertTime)
        {
            PlayAlertSound();
            nextAlertTime = Time.time + timeBetweenAlertSounds;
        }

        timer += Time.deltaTime;
        if (player == null) return;

        float distance = Vector3.Distance(player.position, animator.transform.position);

        // Jumpscare nếu người chơi đến quá gần
        if (distance <= 0.5f)
        {
            animator.SetBool("IsJumpscaring", true);
            //Debug.Log("Direct jumpscare from alert");
            return;
        }

        // ✅ CHỈ QUAY MẶT VỀ PHÍA PLAYER NẾU NHÌN THẤY
        if (CanSeePlayer(animator))
        {
            // Quay mặt về phía người chơi
            Vector3 direction = (player.position - animator.transform.position).normalized;
            animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation,
                Quaternion.LookRotation(direction), Time.deltaTime * 2f);

            // ✅ THÊM CHECK LINE OF SIGHT trước khi chase
            if (distance < chaseRange && timer > 1f)
            {
                animator.SetBool("IsRunning", true);
                //Debug.Log("Switching to chase from alert");
                return;
            }
        }

        // Mất tích người chơi hoặc hết thời gian alert
        if (distance > losePlayerRange || timer > alertDuration)
        {
            animator.SetBool("IsAlert", false);
            //Debug.Log("Alert lost player, returning to idle");
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Exited Alert State");
    }

    // ✅ AUDIO METHODS
    void SetupAudio(Animator animator)
    {
        audioSource = animator.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = animator.gameObject.AddComponent<AudioSource>();
        }

        // Cấu hình 3D sound
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.maxDistance = 25f; // Nghe xa hơn khi alert (đáng sợ hơn)
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void PlayAlertSound()
    {
        if (audioSource != null && alertSound != null)
        {
            audioSource.PlayOneShot(alertSound, alertVolume);
            //Debug.Log("Playing alert sound - Enemy spotted player!");
        }
    }

    // ✅ METHOD MỚI: Check xem có tường chắn không
    bool CanSeePlayer(Animator animator)
    {
        if (player == null) return false;

        float distance = Vector3.Distance(player.position, animator.transform.position);
        Vector3 direction = (player.position - animator.transform.position).normalized;

        // Raycast từ vị trí mắt Agent (thêm Vector3.up để tránh hit ground)
        Vector3 startPos = animator.transform.position + Vector3.up * 1.5f;

        // Raycast để check tường
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, distance, wallLayer))
        {
            // Nếu hit tường trước khi tới Player = có tường chắn
            Debug.DrawRay(startPos, direction * hit.distance, Color.red, 0.1f);
            return false;
        }

        // Không có tường chắn
        Debug.DrawRay(startPos, direction * distance, Color.green, 0.1f);
        return true;
    }
}