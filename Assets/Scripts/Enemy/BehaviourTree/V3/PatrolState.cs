using UnityEngine;
using UnityEngine.AI;

public class PatrolState : StateMachineBehaviour
{
    [Header("Line of Sight")]
    [SerializeField] private LayerMask wallLayer;

    [Header("Field of View")]
    [SerializeField] private float fieldOfViewAngle = 120f;

    [Header("Detection Settings")]
    [SerializeField] private float chaseRange = 20f;

    [Header("Random Patrol Settings")]
    [SerializeField] private float range = 15f;
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip breathingSound;
    [SerializeField] private float minTimeBetweenBreaths = 3f;
    [SerializeField] private float maxTimeBetweenBreaths = 8f;
    [SerializeField] private float breathVolume = 0.7f;

    float timer;
    float waitTimer;
    float waitDuration;
    bool isWaiting = false;
    NavMeshAgent agent;
    Transform player;
    Transform centrePoint;
    private AudioSource audioSource;
    private float nextBreathTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 2.5f;
        timer = 0;
        centrePoint = animator.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SetupAudio(animator);
        ScheduleNextBreath();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Time.time >= nextBreathTime && breathingSound != null)
        {
            PlayBreathingSound();
            ScheduleNextBreath();
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point))
            {
                agent.SetDestination(point);
            }
        }

        timer += Time.deltaTime;

        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance <= 2.5f)
        {
            animator.SetBool("IsJumpscaring", true);
            //Debug.Log("Direct jumpscare from patrol");
            return;
        }

        if (timer > 10)
            animator.SetBool("IsPatrolling", false);

        if (distance < chaseRange && CanSeePlayer(animator))
        {
            animator.SetBool("IsAlert", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
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

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    bool CanSeePlayer(Animator animator)
    {
        if (player == null) return false;

        Vector3 agentPos = animator.transform.position;
        Vector3 playerPos = player.position;

        Vector3 directionToPlayer = (playerPos - agentPos).normalized;
        Vector3 agentForward = animator.transform.forward;
        float angleToPlayer = Vector3.Angle(agentForward, directionToPlayer);

        if (angleToPlayer > fieldOfViewAngle / 2f)
        {
            return false;
        }

        float distance = Vector3.Distance(playerPos, agentPos);
        Vector3 startPos = agentPos + Vector3.up * 1.5f;

        if (Physics.Raycast(startPos, directionToPlayer, out RaycastHit hit, distance, wallLayer))
        {
            return false;
        }

        return true;
    }
}