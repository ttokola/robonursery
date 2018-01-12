using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLAagent : Agent
{
    AgentProto proto;
    public GameObject point;
    public float speed;
    //private Rigidbody rb;
    private Rigidbody body;
    private GameObject b;
    private void Start()
    {
        proto = this.gameObject.GetComponent<AgentProto>();
        //get the first component in the components list.
        b = proto.GetComponents()[0].gameObject;
        // or fetch by name
        //b = proto.GetComponentByName("Body").gameObject;


    }
    public override List<float> CollectState()
    {
       
      //Variables fed to external AI as inputs in state (refer to ML-Agents Python-API documentation) 
        body = b.GetComponent<Rigidbody>();
        List<float> state = new List<float>();
        state.Add(b.transform.position.x);
        state.Add(b.transform.position.z);
        state.Add(b.transform.position.y);
        state.Add(body.velocity.x);
        state.Add(body.velocity.z);
        state.Add(point.transform.position.x);
        state.Add(point.transform.position.z);
        return state;
    }
    /*this function is constantly being executed. Contains a simple reward function that aims to reward standing up-right
    and moving. In tests the robot never learnt to stand up,
    instead pushing arms below its body to stay at an angle avoiding the reset condition*/
    public override void AgentStep(float[] act)
    {
        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            proto.MoveMovableParts(act);
        }

        if (Mathf.Abs(point.transform.position.x - b.transform.position.x) < 1f &&
            Mathf.Abs(point.transform.position.z - b.transform.position.z) < 1f)
        {
            point.transform.position = new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f));
            reward = 100f;
        }
        if (done == false)
        {
            
            reward = body.velocity.magnitude * 0.001f + Mathf.Abs(90 - Mathf.Abs(body.transform.rotation.x)) * 0.0005f + Mathf.Abs(90 - Mathf.Abs(body.transform.rotation.z)) * 0.0005f - Mathf.Sqrt(Mathf.Pow(body.transform.rotation.x, 2));
            Monitor.Log("reward", reward, MonitorType.text);
            Monitor.Log("Act", act, MonitorType.hist);
         

        }

    }
    //When environment is reset this is executed
    public override void AgentReset()
    {
        /*Resets the Agent's GameObject's relative positions and velocities to starting values at a new random offset location*/
        proto.ResetAgentPose(new Vector3(Random.Range(-2f, 2f), 0.01f, Random.Range(-2f, 2f)));
    }

}
