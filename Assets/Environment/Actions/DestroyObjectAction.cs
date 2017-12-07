using BT;
using UnityEngine;

namespace Env
{
    public class DestroyObjectAction : Action
    {
        string tag;

        public DestroyObjectAction(string tag)
        {
            this.tag = tag;
        }

        public override Status Execute()
        {
            GameObject gameObject = GameObject.FindWithTag(tag);

            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }

            return Status.Success;
        }
    }
}
