using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour {

    public GameObject targetobj;
    public Transform targettransform;

    public CarryObject carryObjScript;


    public GameObject lilbot;
    public GameObject nannybot;
    

	// Use this for initialization
	void Start () {
        //carryObjScript.setTask(targetobj, targettransform, 0.0f, new Vector3(0,-1, 0));

        SayHello sh = nannybot.GetComponent<SayHello>();

        sh.setTask(lilbot, 3, true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
