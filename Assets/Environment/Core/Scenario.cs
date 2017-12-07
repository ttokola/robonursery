using BT;
using UnityEngine;

namespace Env
{
    public abstract class Scenario : MonoBehaviour
    {
        public abstract Node Create();
    }
}
