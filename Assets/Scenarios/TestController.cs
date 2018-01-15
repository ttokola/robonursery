using UnityEngine;
using RobotNursery.BT;
using RobotNursery.Environment;

public class TestController : MonoBehaviour
{
    private Sequence schedule;
    private System.Collections.Generic.List<string> scenarioslist = new System.Collections.Generic.List<string>();


    void Start()
    {
        DontDestroyOnLoad(this.transform.gameObject);

        schedule = new Sequence();

        Initialize();

        var scenarios = transform.Find("Scenarios").GetComponents<Scenario>();

        foreach (var scenario in scenarios)
        {
            Node node = scenario.Create();
            schedule.Add(new DebugAction(scenario.GetType().Name + " start"));
            schedule.Add(node);
            schedule.Add(new DebugAction(scenario.GetType().Name + " end"));
        }
    }

    void Initialize()
    {
        Object[] objects = Resources.LoadAll("Scenarios");

        foreach (Object obj in objects)
        {
            Debug.Log(obj.name);
            scenarioslist.Add(obj.name);
        }
        Resources.UnloadUnusedAssets();

        int num = scenarioslist.Count;
        Debug.Log("Number of Scenario files: " + num);

        var list = scenarioslist.ToArray();
        foreach (var item in list)
        {
            transform.Find("Scenarios").gameObject.AddComponent(System.Type.GetType(item));
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
