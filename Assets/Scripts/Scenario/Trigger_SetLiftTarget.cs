using UnityEngine;
using System.Collections;

public class Trigger_SetLiftTarget : MonoBehaviour {

    ScenarioScript scenario;

    public GameObject target;
    LilBotNamespace.PickupObject picking;

    // Use this for initialization
    void Start()
    {
        scenario = transform.parent.GetComponent<ScenarioScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // define events here
    void OnTriggerEnter(Collider other)
    {
        // get root gameobject
        GameObject oth = other.gameObject.transform.root.gameObject;
        picking = oth.GetComponent<LilBotNamespace.PickupObject>();

        //Debug.Log(picking);
        picking.tar = target;
        picking.enabled = true;

        Destroy(gameObject);
    }


}
