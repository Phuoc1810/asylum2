using UnityEngine;
using UnityEngine.AI;

public class IdleState : StateMachineBehaviour
{
    [Header("Idle Settings")]
    [SerializeField] private float idleDuration = 5f;
    [SerializeField] private float detectionRange = 10f;

    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer;

    [Header("Field of View")]
    [SerializeField] private float fieldOfViewAngle = 120f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip breathingSound;
    [SerializeField] private float minTimeBetweenBreaths = 4f;
    [SerializeField] private float maxTimeBetweenBreaths = 10f;
    [SerializeField] private float breathVolume = 0.6f;

    private float timer;
    private Transform player;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private float nextBreathTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null)
        {
            agent.SetDestination(agent.transform.position);
            agent.speed = 0f;
            agent.isStopped = true;
        }

        SetupAudio(animator);
        ScheduleNextBreath();

        //Debug.Log("Entered Idle State");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Time.time >= nextBreathTime && breathingSound != null)
        {
            PlayBreathingSound();
            ScheduleNextBreath();
        }

        timer += Time.deltaTime;

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, animator.transform.position);

            if (distanceToPlayer <= 0.5f)
            {
                animator.SetBool("IsJumpscaring", true);
                return;
            }

            if (distanceToPlayer <= detectionRange && CanSeePlayer(animator))
            {
                animator.SetBool("IsAlert", true);
                return;
            }
        }

        if (timer >= idleDuration)
        {
            animator.SetBool("IsPatrolling", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = 2.5f;
        }

        //Debug.Log("Exited Idle State");
    }

    void SetupAudio(Animator animator)
    {
        audioSource = animator.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = animator.gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = breathVolume;
    }

    void PlayBreathingSound()
    {
        if (audioSource != null && breathingSound != null)
        {
            audioSource.PlayOneShot(breathingSound, breathVolume);
        }
    }

    void ScheduleNextBreath()
    {
        nextBreathTime = Time.time + Random.Range(minTimeBetweenBreaths, maxTimeBetweenBreaths);
    }

    bool CanSeePlayer(Animator animator)
    {
        if (player == null) return false;

        Vector3 agentPos = animator.transform.position;
        Vector3 playerPos = player.position;

        // Kiểm tra góc nhìn (FOV)
        Vector3 directionToPlayer = (playerPos - agentPos).normalized;
        Vector3 agentForward = animator.transform.forward;
        float angleToPlayer = Vector3.Angle(agentForward, directionToPlayer);

        if (angleToPlayer > fieldOfViewAngle / 2f)
        {
            return false;
        }

        // Kiểm tra tường chắn
        float distance = Vector3.Distance(playerPos, agentPos);
        Vector3 startPos = agentPos + Vector3.up * 1.5f;

        if (Physics.Raycast(startPos, directionToPlayer, out RaycastHit hit, distance, wallLayer))
        {
            return false;
        }

        return true;
    }
}