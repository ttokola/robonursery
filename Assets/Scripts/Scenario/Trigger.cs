using UnityEngine;
using System.Collections;

// trigger template
public class Trigger : MonoBehaviour {

    ScenarioScript scenario;

    // Use this for initialization
    void Start () {
        scenario = transform.parent.GetComponent<ScenarioScript>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // define events here
    void OnTriggerEnter(Collider other)
    {
        // do things
        //scenario.testCall();

        // destroy trigger
        Destroy(gameObject);
    }

}
