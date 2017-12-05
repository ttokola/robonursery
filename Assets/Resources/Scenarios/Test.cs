using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, IScenario {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void EnableScenario(bool enabled) { return; }
    public void ResetScenario() { }
    public string[] GetRequirements() { return new string[]{ }; }
}
