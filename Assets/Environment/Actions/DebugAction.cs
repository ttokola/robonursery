using RobotNursery.BT;
using UnityEngine;

namespace RobotNursery.Environment
{
    public class DebugAction : Action
    {
        private string message;

        public DebugAction(string message)
        {
            this.message = message;
        }

        public override BT.Status Execute()
        {
            Debug.LogFormat(message);
            return BT.Status.Success;
        }
    }
}