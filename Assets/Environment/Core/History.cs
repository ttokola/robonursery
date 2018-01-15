using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;

namespace RobotNursery.Environment
{
    public class History : MonoBehaviour
    {
        public int MaxSize = 5;

        public struct Trial
        {
            public double Reward;
            public bool Complete;
        }

        private Dictionary<Scenario, List<Trial>> Rewards
            = new Dictionary<Scenario, List<Trial>>();

        public void BeginTrial(Scenario scenario)
        {
            if (MaxSize <= 0)
            {
                return;
            }

            List<Trial> trials;
            bool complete = true;

            if (Rewards.TryGetValue(scenario, out trials))
            {
                Assert.IsTrue(trials.Count > 0);
                var last = trials.Last();
                if (!last.Complete)
                {
                    complete = false;
                }
            }
            else
            {
                trials = new List<Trial>();
                Rewards[scenario] = trials;
            }

            if (complete)
            {
                if (trials.Count >= MaxSize)
                {
                    trials.RemoveAt(0);
                }
                trials.Add(new Trial());
            }
        }

        public void EndTrial(Scenario scenario)
        {
            List<Trial> trials;

            if (Rewards.TryGetValue(scenario, out trials))
            {
                Assert.IsTrue(trials.Count > 0);
                var last = trials.Last();
                if (!last.Complete)
                {
                    last.Complete = true;
                    trials[trials.Count - 1] = last;
                }
            }
        }

        public IEnumerable<Trial> GetTrials(Scenario scenario)
        {
            List<Trial> trials;
            if (Rewards.TryGetValue(scenario, out trials))
            {
                return trials;
            }
            else
            {
                return null;
            }
        }

        public void AddReward(Scenario scenario, double reward)
        {
            if (!TryAddReward(scenario, reward))
            {
                throw new InvalidOperationException(
                    "Could not add reward, make sure to call BeginTrial first");
            }
        }

        private bool TryAddReward(Scenario scenario, double reward)
        {
            List<Trial> trials;

            if (Rewards.TryGetValue(scenario, out trials))
            {
                Assert.IsTrue(trials.Count > 0);
                var last = trials.Last();
                if (!last.Complete)
                {
                    last.Reward += reward;
                    trials[trials.Count - 1] = last;
                    return true;
                }
            }

            return false;
        }
    }
}
