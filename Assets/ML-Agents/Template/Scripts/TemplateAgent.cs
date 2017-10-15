using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateAgent : Agent {

    public GameObject body;

    private Vector3 destionation = new Vector3(-28, 0, -6);

	public override List<float> CollectState()
	{
		List<float> state = new List<float>();

        state.Add(body.transform.position.x);
        state.Add(body.transform.position.z);

        return state;
	}

	public override void AgentStep(float[] act)
	{
        int action = Mathf.FloorToInt(act[0]);

        switch (action)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
	}

	public override void AgentReset()
	{

	}

	public override void AgentOnDone()
	{

	}
}
