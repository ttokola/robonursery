using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class MLAagent : Agent {
    AgentProto proto;
    /*
    private Vector3 shoulderForce;

    private Rigidbody LUpperArm_RB;

    private Transform LUpperArm_tf;

    private Rigidbody leftWheel_RB;
    private Transform leftWheel_tf;

    private Rigidbody rightWheel_RB;
    private Transform rightWheel_tf;

    private MovementControls motor;
    */
    public GameObject point;
    private Rigidbody rb;
    private GameObject LHand;
    private GameObject RHand;


    private void Start()
    {
        proto = this.gameObject.GetComponent<AgentProto>();
        //rb = GetComponent<Rigidbody>();
        LHand = proto.GetComponentByName("LHand").gameObject;
        RHand = proto.GetComponentByName("RHand").gameObject;

    }

    public override List<float> CollectState()
	{
		List<float> state = new List<float>();

        state.Add(gameObject.transform.position.x);
        state.Add(gameObject.transform.position.y);
        state.Add(gameObject.transform.position.z);
        //state.Add(gameObject.transform.GetComponent<Rigidbody>().velocity.x / 5f);
       //state.Add(gameObject.transform.GetComponent<Rigidbody>().velocity.z / 5f);
        state.Add(point.transform.position.x);
        state.Add(point.transform.position.z);
        return state;
	}
    //This is called every frame.
    //Currently only works if brain is set to discrete. 
    public override void AgentStep(float[] act)
    {


        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            proto.MoveMovableParts(act);
        }

        if (Mathf.Abs(point.transform.position.x - gameObject.transform.position.x) < 1f &&
            Mathf.Abs(point.transform.position.z - gameObject.transform.position.z) < 1f)
        {
            done = true;
            reward = 1f;
        }
        if (done == false)
        {
            //if (rb.velocity.magnitude != 0f) { reward = 0.1f; } else { done = true; reward = -1f; }
        }
    }

	public override void AgentReset()
	{
      //  gameObject.transform.position = new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));
      proto.ResetAgentPose();
    }

    public override void AgentOnDone()
	{
       
    }
}
