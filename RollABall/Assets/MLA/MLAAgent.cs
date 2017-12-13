using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLAAgent : Agent
{
    AgentProto proto;
    public GameObject point;
    public float speed;
    //private Rigidbody rb;
    private Rigidbody body;
    
    private void Start()
    {
        proto = this.gameObject.GetComponent<AgentProto>();
        body = proto.GetComponentByName("Body").gameObject.GetComponent<Rigidbody>();
        //rb = GetComponent<Rigidbody>();
        

    }
    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        state.Add(gameObject.transform.position.x);
        state.Add(gameObject.transform.position.z);
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
        
        if (Mathf.Abs(point.transform.position.x - gameObject.transform.position.x)<1f &&
            Mathf.Abs(point.transform.position.z - gameObject.transform.position.z)<1f)
        {
            done = true;
            reward = 1f;
        }
        if (done == false)
        {
            if (body.velocity.magnitude >= 1f && gameObject.transform.position.y < 2f) { reward = 0.001f; } // else { done = true; reward = -0.1f; }
            if (Mathf.Abs(gameObject.transform.position.y) >= 2f) { done = true; reward = -1f; };
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
        body.MovePosition(new Vector3(Random.Range(-2f, 2f), 1.0f, Random.Range(-2f, 2f)));
        //body.isKinematic = false;
    }

}