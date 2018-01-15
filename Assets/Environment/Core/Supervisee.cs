using UnityEngine;

namespace RobotNursery.Environment
{
    [RequireComponent(typeof(History))]
    public partial class Supervisee : MonoBehaviour
    {
        private History history;

        private void Awake()
        {
            history = GetComponent<History>();
        }

        public void GiveReward(Scenario scenario, double reward)
        {
            history.AddReward(scenario, reward);
            OnReward(scenario, reward);
        }
    }

    public partial class Supervisee : MonoBehaviour
    {
        public virtual void OnReward(Scenario scenario, double reward) { }
    }
}
