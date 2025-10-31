using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AGENT : StateMachineBehaviour
{
    NavMeshAgent agent;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 30f;
        
    }
}
