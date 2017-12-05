using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour {

    public GameObject targetobj;
    public Transform targettransform;

    public Transform throwtargettransform;

    public CarryObject carryObjScript;

    


    public GameObject lilbot;
    public GameObject nannybot;

    public GameObject ballobject;
    

	// Use this for initialization
	void Start () {
        //carryObjScript.setTask(targetobj, targettransform, 0.0f, new Vector3(0,-1, 0));

        //SayHello sh = nannybot.GetComponent<SayHello>();

        //sh.setTask(lilbot, 3, true);

        ThrowObject to = nannybot.GetComponent<ThrowObject>();

        to.setTask(ballobject, targettransform, throwtargettransform, 0.0f, new Vector3(0.2f, 0.1f, 0.3f));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
