using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour {

    public GameObject targetobj;
    public Transform targettransform;

    public CarryObject carryObjScript;
    

	// Use this for initialization
	void Start () {
        //carryObjScript.setTask(targetobj, targettransform, 0.0f, new Vector3(0,-1, 0));

        GameObject.Find("Spawn_Livingroom").GetComponent<GetSpawnPoint>().getSpawnPoint();

	}
	
	// Update is called once per frame
	void Update () {
		


	}
}
