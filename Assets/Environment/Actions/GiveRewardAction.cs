using RobotNursery.BT;
using UnityEngine;

namespace RobotNursery.Environment
{
    public class GiveRewardAction : Action
    {
        private Supervisee supervisee;
        private Scenario scenario;
        private double reward;

        public GiveRewardAction(
            Supervisee supervisee,
            Scenario scenario,
            double reward)
        {
            this.supervisee = supervisee;
            this.scenario = scenario;
            this.reward = reward;
        }

        public override Status Execute()
        {
            supervisee.GiveReward(scenario, reward);
            return Status.Success;
        }
    }
}
