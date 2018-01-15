using UnityEngine;

namespace RobotNursery.Environment
{
    public class NoticeTargetObjective : Objective
    {
        private GameObject gameObject;
        private string targetTag;
        private bool done = false;

        public NoticeTargetObjective(GameObject gameObject, string targetTag)
        {
            this.gameObject = gameObject;
            this.targetTag = targetTag;
        }

        override protected Progress Check()
        {
            var position = gameObject.transform.position;
            var forward = gameObject.transform.forward;
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