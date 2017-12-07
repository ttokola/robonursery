using UnityEngine;
using BT;
using Env;

public class TestController : MonoBehaviour
{
    private Sequence schedule;

    void Start()
    {
        schedule = new Sequence();

        var scenarios = transform.Find("Scenarios").GetComponents<Scenario>();

        foreach (var scenario in scenarios)
        {
            Node node = scenario.Create();
            schedule.Add(new DebugAction(scenario.GetType().Name + " start"));
            schedule.Add(node);
            schedule.Add(new DebugAction(scenario.GetType().Name + " end"));
        }
    }

    void Update()
    {
        if (schedule != null)
        {
            schedule.Tick();
        }
    }
}
