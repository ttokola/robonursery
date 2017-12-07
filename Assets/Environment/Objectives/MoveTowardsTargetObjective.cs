using UnityEngine;

namespace Env
{
    public class MoveTowardsTargetObjective : Objective
    {
        private GameObject gameObject;
        private GameObject targetObject;
        private string targetTag;
        private bool done = false;

        public MoveTowardsTargetObjective(GameObject gameObject, string targetTag)
        {
            this.gameObject = gameObject;
            this.targetTag = targetTag;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            targetObject = GameObject.FindGameObjectWithTag(targetTag);
        }

        protected override void OnLeave()
        {
            targetObject = null;
            base.OnLeave();
        }

        protected override Progress Check()
        {
            if (targetObject != null)
            {
                double distance = Vector3.Distance(
                    gameObject.transform.position,
                    targetObject.transform.position);

                /*Debug.LogFormat("[{0}] Check distance={1}",
                    GetType().Name, distance);*/

                if (distance < 2.0)
                {
                    return Progress.COMPLETE;
                }
            }

            return Progress.INCOMPLETE;
        }
    }
}