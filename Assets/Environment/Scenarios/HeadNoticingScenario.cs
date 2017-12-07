using BT;
using UnityEngine;

namespace Env
{
    public class HeadNoticingScenario : Scenario
    {
        public GameObject head;

        public override Node Create()
        {
            // Prepare environment actions

            Action spawnTargetAction = new SpawnBallAction(
                "TargetBall", new Vector3(4.14f, 1.38f, 2.00f));
            Action destroyTargetAction = new DestroyObjectAction(
                "TargetBall");

            // Prepare agent objectives

            Objective noticeTargetObjective =
                new NoticeTargetObjective(head, "TargetBall");

            // Construct scenario

            Sequence sequence = new Sequence();
            sequence.Add(spawnTargetAction);
            sequence.Add(noticeTargetObjective);
            sequence.Add(destroyTargetAction);

            return sequence;
        }
    }
}