using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RoboAgent : Agent
{
    public GameObject Head;
    //Rigidbody rBody;
    void Start()
    {
        //rBody = GetComponent<Rigidbody>();
    }


    public override void AgentReset()
    {
       
    }

    public override void CollectObservations()
    {
        // ADD ALL (NORMALIZED) OBSERVATIONS HERE
        AddVectorObs(Head.transform.rotation);
        AddVectorObs(Head.GetComponent<Rigidbody>().angularVelocity);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Head.GetComponent<Rigidbody>().AddTorque(new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]));
        Head.GetComponent<Rigidbody>().AddTorque(Head.transform.right* vectorAction[0]);
        Head.GetComponent<Rigidbody>().AddTorque(Head.transform.up * vectorAction[1]);
        Head.GetComponent<Rigidbody>().AddTorque(Head.transform.forward* vectorAction[2]);
    }
}
