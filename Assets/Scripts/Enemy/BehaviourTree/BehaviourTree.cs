//using NUnit.Framework;
//using System.Collections;
//using System.Runtime.InteropServices.WindowsRuntime;
//using UnityEngine;
//using UnityEngine.AI;

//public enum StateNode
//{
//    Sucess,
//    Fail,
//    Running
//}
//public abstract class btNode
//{
//    public abstract StateNode evaluate();

//}
//public class selector : btNode
//{
//    public override StateNode evaluate()
//    {
//        foreach(btNode node in nodes)
//        {
//            StateNode state = node.evaluate();
//            if(state == StateNode.Sucess || state == StateNode.Running)
//            {
//                return state;
//            }

//        }
//        return StateNode.Fail;
//    }
//    public btNode[] nodes;
//    public selector(btNode[] nodes)
//    {
//        this.nodes = nodes;
//    }   
//}
//    public class sequence:btNode
//{

//    public btNode[] nodes;
//    public sequence(btNode[] nodes)
//    {
//        this.nodes = nodes;
//    }

//    public override StateNode evaluate()
//    {
//        foreach (btNode node in nodes)
//        {
//            StateNode states = node.evaluate();
//            if(states == StateNode.Fail)
//            {
//                return StateNode.Fail;
//            }
//            if(states == StateNode.Running)
//            {
//                return StateNode.Running;
//            }
//        }
//        return StateNode.Sucess;

//    }
//}
//public class CheckPlayerInRange : btNode
//{
//    private Transform ai, player;
//    private float range;

//    public CheckPlayerInRange(Transform ai, Transform player, float range)
//    {
//        this.ai = ai;
//        this.player = player;
//        this.range = range;
//    }

//    public override StateNode evaluate()
//    {
//        float dist = Vector3.Distance(ai.position, player.position);
//        return dist <= range ? StateNode.Sucess : StateNode.Fail;
//    }
//}
//public class ChasePlayer : btNode
//{
//    private NavMeshAgent agent;
//    private Transform player;
//    private Animator animator;
//    private string runAnim;
//    private int animLayer;
//    private bool playedRunOnce = false;

//    private float repathInterval = 0.1f;
//    private float repathTimer = 0f;

//    public ChasePlayer(
//        NavMeshAgent agent,
//        Transform player,
//        Animator animator = null,
//        string runAnim = "Run",
//        int animLayer = 0
//    )
//    {
//        this.agent = agent;
//        this.player = player;
//        this.animator = animator;
//        this.runAnim = runAnim;
//        this.animLayer = animLayer;
//    }

//    public override StateNode evaluate()
//    {
//        if (agent == null || player == null || !agent.enabled)
//            return StateNode.Fail;

//        if (animator != null && !playedRunOnce)
//        {
//            PlayIfNotCurrent(runAnim);
//            playedRunOnce = true;
//        }

//        repathTimer -= Time.deltaTime;
//        if (repathTimer <= 0f || Vector3.Distance(agent.transform.position, player.position) > 1f)
//        {
//            if (NavMesh.SamplePosition(player.position, out var hit, 2.0f, NavMesh.AllAreas))
//                agent.SetDestination(hit.position);
//            repathTimer = repathInterval;
//        }

//        float distance = Vector3.Distance(agent.transform.position, player.position);
//        if (distance <= 0.6f)
//        {
//            agent.isStopped = true;
//            agent.velocity = Vector3.zero;
//            return StateNode.Sucess;
//        }

//        if (agent.pathPending) return StateNode.Running;
//        return StateNode.Running;
//    }

//    private void PlayIfNotCurrent(string stateName)
//    {
//        if (animator == null || string.IsNullOrEmpty(stateName)) return;

//        var st = animator.GetCurrentAnimatorStateInfo(animLayer);
//        if (!st.IsName(stateName))
//            animator.Play(stateName, animLayer, 0f);
//    }
//}
//public class Patrol : btNode
//{
//    private NavMeshAgent agent;
//    private Transform[] points;

//    private Animator animator;
//    private string walkAnim;
//    private string idleAnim;
//    private int animLayer;

//    private int index = -1;
//    private bool started = false;

//    // ---- Tùy chọn chống "chớp" ----
//    private float arriveSpeedThreshold = 0.05f; // tốc độ gần như đứng yên mới coi là tới
//    private float hysteresisTime = 0.05f;       // cần đứng yên ít khung hình (chống jitter)
//    private float arrivedStillTime = 0f;

//    // ---- (Tùy chọn) dừng/Idle ngẫu nhiên tại waypoint ----
//    private bool pauseAtWaypoint = false; // để mặc định false: không phát Idle ở góc
//    private float pauseMin = 0.8f, pauseMax = 1.8f;
//    private float pauseEndTime = 0f;
//    private bool isPaused = false;

//    public Patrol(
//        NavMeshAgent agent,
//        Transform[] points,
//        Animator animator,
//        string walkAnim = "Walk",
//        string idleAnim = "Idle",
//        int animLayer = 0,
//        bool pauseAtWaypoint = false,      // ← bật nếu bạn muốn dừng
//        float pauseMin = 0.8f,
//        float pauseMax = 1.8f
//    )
//    {
//        this.agent = agent;
//        this.animator = animator;
//        this.walkAnim = walkAnim;
//        this.idleAnim = idleAnim;
//        this.animLayer = animLayer;

//        this.pauseAtWaypoint = pauseAtWaypoint;
//        this.pauseMin = Mathf.Max(0f, pauseMin);
//        this.pauseMax = Mathf.Max(this.pauseMin, pauseMax);

//        // Lọc null waypoint
//        if (points != null)
//        {
//            var list = new System.Collections.Generic.List<Transform>();
//            foreach (var p in points) if (p != null) list.Add(p);
//            this.points = list.ToArray();
//        }
//        else this.points = System.Array.Empty<Transform>();
//    }

//    public override StateNode evaluate()
//    {
//        if (agent == null || !agent.enabled || points == null || points.Length == 0)
//            return StateNode.Fail;

//        if (!started)
//        {
//            index = 0;
//            SetNext();
//            started = true;
//            PlayIfNotCurrent(walkAnim);
//            return StateNode.Running;
//        }

//        // Đang pause tại waypoint?
//        if (isPaused)
//        {
//            if (Time.time >= pauseEndTime)
//            {
//                isPaused = false;
//                if (agent.isStopped) agent.isStopped = false;
//                NextPointAndGo();
//            }
//            return StateNode.Running;
//        }

//        // Điều kiện coi là "đã đến" (có hysteresis chống nháy)
//        bool closeEnough = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
//        if (closeEnough)
//        {
//            // nếu tốc độ nhỏ hơn ngưỡng trong một khoảng rất ngắn → thật sự tới
//            if (agent.velocity.sqrMagnitude < arriveSpeedThreshold * arriveSpeedThreshold)
//            {
//                arrivedStillTime += Time.deltaTime;
//                if (arrivedStillTime >= hysteresisTime)
//                {
//                    if (pauseAtWaypoint)
//                    {
//                        // Pause: dừng và Idle
//                        isPaused = true;
//                        pauseEndTime = Time.time + Random.Range(pauseMin, pauseMax);
//                        if (!agent.isStopped) agent.isStopped = true;
//                        PlayIfNotCurrent(idleAnim);
//                        return StateNode.Running;
//                    }
//                    else
//                    {
//                        // Không pause: chuyển waypoint ngay, KHÔNG phát Idle để tránh nháy
//                        NextPointAndGo();
//                        return StateNode.Running;
//                    }
//                }
//            }
//            else
//            {
//                // còn đang quay/nhích → reset bộ đếm
//                arrivedStillTime = 0f;
//            }
//        }
//        else
//        {
//            arrivedStillTime = 0f;
//        }

//        // Đang di chuyển → giữ Walk
//        if (agent.velocity.sqrMagnitude > 0.01f)
//            PlayIfNotCurrent(walkAnim);

//        return StateNode.Running;
//    }

//    private void NextPointAndGo()
//    {
//        index = (index + 1) % points.Length;
//        SetNext();
//        if (agent.isStopped) agent.isStopped = false;
//        PlayIfNotCurrent(walkAnim);
//    }

//    private void SetNext()
//    {
//        var target = points[index].position;
//        if (NavMesh.SamplePosition(target, out var hit, 2.0f, NavMesh.AllAreas))
//            agent.SetDestination(hit.position);
//    }

//    private void PlayIfNotCurrent(string stateName)
//    {
//        if (animator == null || string.IsNullOrEmpty(stateName)) return;

//        var st = animator.GetCurrentAnimatorStateInfo(animLayer);
//        if (!st.IsName(stateName))
//            animator.Play(stateName, animLayer, 0f);
//    }
//}

//public class CloseToPlayer : btNode
//{
//    private Transform ai;
//    private Transform player;
//    private float closeRange;

//    public CloseToPlayer(Transform ai, Transform player, float closeRange = 0.5f) // Đổi thành 0.5m
//    {
//        this.ai = ai;
//        this.player = player;
//        this.closeRange = closeRange;
//    }

//    public override StateNode evaluate()
//    {
//        float dist = Vector3.Distance(ai.position, player.position);
//        return dist <= closeRange ? StateNode.Sucess : StateNode.Fail;
//    }
//}

//public class Jumpscare : btNode
//{
//    private NavMeshAgent agent;
//    private Animator animator;
//    private string jumpscareTrigger;
//    private bool started = false;
//    private Transform playerCamera;
//    private Transform ai;
//    private float rotationTime = 0.3f;
//    private float rotationElapsed = 0f;

//    public Jumpscare(NavMeshAgent agent, Animator animator, Transform ai, Transform playerCamera, string jumpscareTrigger = "Jumpscare")
//    {
//        this.agent = agent;
//        this.animator = animator;
//        this.jumpscareTrigger = jumpscareTrigger;
//        this.playerCamera = playerCamera;
//        this.ai = ai;
//    }

//    public override StateNode evaluate()
//    {
//        if (!started)
//        {
//            animator.Play(jumpscareTrigger);
//            // Vô hiệu hóa CharacterController của player
//            CharacterController playerController = ai.GetComponentInParent<CharacterController>();
//            if (playerController != null)
//            {
//                playerController.enabled = false; // Khóa di chuyển player
//            }
//            started = true;
//            rotationElapsed = 0f;
//            return StateNode.Running;
//        }

//        if (rotationElapsed < rotationTime)
//        {
//            rotationElapsed += Time.deltaTime;
//            Vector3 lookDir = (ai.position - playerCamera.position).normalized;
//            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
//            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, rotationElapsed / rotationTime);
//            return StateNode.Running;
//        }

//        if (animator.GetCurrentAnimatorStateInfo(0).IsName(jumpscareTrigger))
//        {
//            return StateNode.Running;
//        }

//        // Kết thúc jumpscare, kích hoạt lại CharacterController
//        if (rotationElapsed >= rotationTime && !animator.GetCurrentAnimatorStateInfo(0).IsName(jumpscareTrigger))
//        {
//            CharacterController playerController = ai.GetComponentInParent<CharacterController>();
//            if (playerController != null)
//            {
//                playerController.enabled = true; // Mở khóa di chuyển player
//            }
//            return StateNode.Sucess;
//        }

//        return StateNode.Running;
//    }
//}
