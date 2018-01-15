using RobotNursery.BT;
using UnityEngine;

namespace RobotNursery.Environment
{
    public abstract class Scenario : MonoBehaviour
    {
        public abstract Node Create();
    }
}