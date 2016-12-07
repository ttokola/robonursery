using UnityEngine;
using System.Collections;

public class Trigger_AfterLift : MonoBehaviour {

    ScenarioScript scenario;

    // Use this for initialization
    void Start()
    {
        scenario = transform.parent.GetComponent<ScenarioScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 target = new Vector3(1.0f, 0.0f, 2.0f);
        int aliceIndex = scenario.nameDict["Alice"];
        scenario.startMoveTo(scenario.botList[aliceIndex], target);
        scenario.testCall();
        Destroy(gameObject);
    }

}
