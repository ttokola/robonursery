using UnityEngine;

namespace RobotNursery.Environment
{
    public class MoveTowardsTargetObjective : Objective
    {
        private Supervisee supervisee;
        private GameObject targetObject;
        private string targetTag;
        private bool done = false;

        public MoveTowardsTargetObjective(Supervisee supervisee, string targetTag)
        {
            this.supervisee = supervisee;
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
                    supervisee.transform.position,
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