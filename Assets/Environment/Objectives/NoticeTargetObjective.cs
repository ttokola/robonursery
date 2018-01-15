using UnityEngine;

namespace RobotNursery.Environment
{
    public class NoticeTargetObjective : Objective
    {
        private Supervisee supervisee;
        private string targetTag;

        public NoticeTargetObjective(Supervisee supervisee, string targetTag)
        {
            this.supervisee = supervisee;
            this.targetTag = targetTag;
        }

        override protected Progress Check()
        {
            var head = supervisee.transform.Find("Head");
            var position = head.transform.position;
            var forward = head.transform.forward;
            var ray = new Ray(position, forward);
            RaycastHit hit;

            Debug.DrawRay(position, forward * 10);

            if (Physics.Raycast(ray, out hit, 10.0f))
            {
                Debug.Log("Raycast hit " + hit.collider.name);

                if (hit.collider.tag == targetTag)
                {
                    return Progress.COMPLETE;
                }
            }

            return Progress.INCOMPLETE;
        }
    }
}