using UnityEngine;

public enum NoteState
{
    Success,
    Running,
    Failure
}
public abstract class BehaviourNode
{
    public abstract NoteState Evalute();
}
public class BehaviourTreeRunner: MonoBehaviour
{
    protected BehaviourNode rootNode;

    protected virtual void Update()
    {
        rootNode?.Evalute();
    }

    protected void SetRootNode(BehaviourNode root)
    {
        rootNode = root;
    }
}