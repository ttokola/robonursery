using BT;
using UnityEngine;

namespace Env
{
    public class FollowingScenario : Scenario
    {
        public GameObject head;

        public override Node Create()
        {
            // Prepare environment actions

            Action spawnTargetAction = new SpawnBallAction(
                "TargetBall", new Vector3(4.14f, 1.38f, 1.78f));
            Action destroyTargetAction = new DestroyObjectAction(
                "TargetBall");

            // Prepare agent objectives

            Objective noticeTargetObjective =
                new NoticeTargetObjective(head, "TargetBall");
            Objective moveTowardsTargetObjective =
                new MoveTowardsTargetObjective(head, "TargetBall");

            // Construct scenario

            Parallel parallel = new Parallel();
            parallel.Add(noticeTargetObjective);
            parallel.Add(moveTowardsTargetObjective);

            Sequence sequence = new Sequence();
            sequence.Add(spawnTargetAction);
            sequence.Add(parallel);
            sequence.Add(destroyTargetAction);

            return sequence;
        }
    }
}
