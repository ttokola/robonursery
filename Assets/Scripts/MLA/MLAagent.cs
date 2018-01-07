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
        b = proto.GetComponentByName("Body").gameObject;
        body = b.GetComponent<Rigidbody>();
        //head = proto.GetComponentByName("Head").gameObject;
        //rb = GetComponent<Rigidbody>();


    }
    public override List<float> CollectState()
    {
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

    public override void AgentStep(float[] act)
    {
        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            proto.MoveMovableParts(act);
            /*float moveHorizontal = act[0];
            float moveVertical = act[1];
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.AddForce(movement * speed);*/
        }

        if (Mathf.Abs(point.transform.position.x - b.transform.position.x) < 1f &&
            Mathf.Abs(point.transform.position.z - b.transform.position.z) < 1f)
        {
            point.transform.position = new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f));
            reward = 100f;
        }
        if (done == false)
        {
            //if (body.velocity.magnitude >= 1f && gameObject.transform.position.y < 2f) { reward = 0.1f; } else { done = true; reward = -0.1f; }
            reward = body.velocity.magnitude * 0.001f + Mathf.Abs(90 - Mathf.Abs(body.transform.rotation.x)) * 0.0005f + Mathf.Abs(90 - Mathf.Abs(body.transform.rotation.z)) * 0.0005f - Mathf.Sqrt(Mathf.Pow(body.transform.rotation.x, 2));
            Monitor.Log("reward", reward, MonitorType.text);
            Monitor.Log("Act", act, MonitorType.hist);
            //if (Mathf.Abs(gameObject.transform.position.y) >= 2f) { done = true; reward = -1f; };
            /*if (Mathf.Abs(body.transform.rotation.x) >= 75f || Mathf.Abs(body.transform.rotation.z) >= 25f || body.velocity.magnitude <= 0.1f)
            {
                reward = -1f;
                done = true;
            }*/

        }

    }

    public override void AgentReset()
    {
        //
        //Instantiate(brick, new Vector3(x, y, 0), Quaternion.identity);
        //Instantiate(gameObject.transform, new Vector3(Random.Range(-2f, 2f), 1.0f, Random.Range(-2f, 2f)), Quaternion.identity);
        //https://docs.unity3d.com/ScriptReference/Rigidbody-isKinematic.html otherwise the child rigidbodies will prevent teleportation
        //body.isKinematic = true;
        //gameObject.transform.position = new Vector3(Random.Range(-2f, 2f), 1.0f, Random.Range(-2f, 2f));
        //body.MovePosition(new Vector3(Random.Range(-2f, 2f), 1.0f, Random.Range(-2f, 2f)));
        //body.isKinematic = false;
        proto.ResetAgentPose(new Vector3(Random.Range(-2f, 2f), 0.01f, Random.Range(-2f, 2f)));
    }

}