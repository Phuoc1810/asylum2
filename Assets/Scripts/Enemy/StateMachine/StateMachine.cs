using Unity.VisualScripting;
using UnityEngine;

public class StateMachine: MonoBehaviour
{
    public IState currentState;

    public void ChangeState(IState newState)
    {
        if (currentState != null && currentState == newState) return;

        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
}