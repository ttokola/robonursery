using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Env;

public class DynamicScenarioLoadTest {

	[Test]
	public void DynamicScenarioLoadTestSimplePasses() {
        // Use the Assert class to test conditions.
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        TestController tc = gameController.GetComponent<TestController>();
        Assert.AreNotEqual(tc, null, "tc is null");
        tc.Initialize();

        Object[] objects = Resources.LoadAll("Scenarios");

        System.Collections.Generic.List<string> scenarioslist = new System.Collections.Generic.List<string>();

        foreach (Object obj in objects)
        {
            //Debug.Log(obj.name);
            scenarioslist.Add(obj.name);
        }
        Resources.UnloadUnusedAssets();

        int i = 0;

        Assert.AreEqual(scenarioslist.Count, tc.GetScenarios().Count, "Count not same!");
        Assert.AreEqual(scenarioslist.Count, gameController.transform.Find("Scenarios").GetComponents<Scenario>().Length, "Children component Count not same!");

        Debug.Log("Test starts");
        foreach (var item in scenarioslist)
        {
            
            Debug.Log(tc.GetScenarios()[i]);
            Debug.Log(item);
            UnityEngine.Assertions.Assert.AreEqual(tc.GetScenarios()[i], item, "Not all scenarios were dynamically found!");
            i++;
        }
        
    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator DynamicScenarioLoadTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
