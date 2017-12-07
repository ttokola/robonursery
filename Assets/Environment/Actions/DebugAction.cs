using BT;
using UnityEngine;

public class DebugAction : Action
{
    private string message;

    public DebugAction(string message)
    {
        this.message = message;
    }

    public override Status Execute()
    {
        Debug.LogFormat(message);
        return Status.Success;
    }
}
