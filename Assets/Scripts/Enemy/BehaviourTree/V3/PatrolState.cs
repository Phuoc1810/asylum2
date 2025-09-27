using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrolState : StateMachineBehaviour
{
    float timer;
    List<Transform> patrolList = new List<Transform>();
    NavMeshAgent agent;
    Transform player;
    float chaseRange = 10;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 2.5f;
        timer = 0;
        GameObject go = GameObject.FindGameObjectWithTag("Patrol point");
        foreach (Transform t in go.transform) 
        patrolList.Add(t);

        agent.SetDestination(patrolList[Random.Range(0, patrolList.Count)].position);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
            agent.SetDestination(patrolList[Random.Range(0, patrolList.Count)].position);
        timer += Time.deltaTime;
        if (timer > 10)
            animator.SetBool("IsPatrolling", false);

        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < chaseRange)
            animator.SetBool("IsAlert", true);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
    }
}
