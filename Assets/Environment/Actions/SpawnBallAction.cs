using RobotNursery.BT;
using UnityEngine;

namespace RobotNursery.Environment
{
    public class SpawnBallAction : Action
    {
        string tag;
        Vector3 position;

        public SpawnBallAction(string tag, Vector3 position)
        {
            this.tag = tag;
            this.position = position;
        }

        public override Status Execute()
        {
            GameObject gameObject = GameObject.FindWithTag(tag);

            if (gameObject == null)
            {
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                gameObject.transform.position = position;
                gameObject.tag = tag;
            }

            return Status.Success;
        }
    }
}