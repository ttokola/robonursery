using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
public class RollerAgent : Agent{

    Rigidbody rBody;
    public Transform Target;
    public float speed = 10;
    private float previousDistance = float.MaxValue;


    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
        previousDistance = float.MaxValue;
    }

    public override void AgentReset()
    {
        if (transform.position.y < -1)
        {
            // RollerBall has fallen
            this.transform.position = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
        }
        else
        {
            // Move the target to a new spot
            Target.position = new Vector3(Random.value * 8 - 4,
                                          0.5f,
                                          Random.value * 8 - 4);
        }
    }

    public override void CollectObservations()
    {
        // Collect observation
        // All the values are divided by 5 to normalize the inputs to the neural network 
        // to the range [-1,1]. (The number five is used because the platform is 10 units across.)

        // Calculate relative position
        var relativePosition = Target.position - transform.position;
        // Relative position
        AddVectorObs(relativePosition.x / 5);
        AddVectorObs(relativePosition.z / 5);

        // Distance to edges of platform
        AddVectorObs((this.transform.position.x + 5) / 5);
        AddVectorObs((this.transform.position.x - 5) / 5);
        AddVectorObs((this.transform.position.z + 5) / 5);
        AddVectorObs((this.transform.position.z - 5) / 5);

        // Ball Velocity
        AddVectorObs(rBody.velocity.x / 5);
        AddVectorObs(rBody.velocity.z / 5);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float distanceToTarget = Vector3.Distance(transform.position, Target.position);

        // Target reached
        if(distanceToTarget < 1.5f)
        {
            AddReward(1.0f);
            Done();
        }

        // Time penalty
        AddReward(-0.01f);

        // Fell off platform
        if(transform.position.y < -1.0f)
        {
            AddReward(-1.0f);
            Done();
        }
                
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);
    }
}
