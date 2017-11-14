using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class MLAagent : Agent {

    

    public MovementControls Move;
    public BallJoint Arms;
    public ExampleControl example;

    public float leftWheel;
    public float rightWheel;

    public float leftShoulderX;
    public float leftShoulderY;
    public float leftShoulderZ;

    public float rightShoulderX;
    public float rightShoulderY;
    public float rightShoulderZ;

    public float leftElbowX;
    public float leftElbowY;
    public float leftElbowZ;

    public float rightEbowX;
    public float rightElbowY;
    public float rightElbowZ;
    private Vector3 shoulderForce;

    private Rigidbody LUpperArm_RB;

    private Transform LUpperArm_tf;

    private Rigidbody leftWheel_RB;
    private Transform leftWheel_tf;

    private Rigidbody rightWheel_RB;
    private Transform rightWheel_tf;


    public void MoveArm(Rigidbody rb, Transform tf, float force, string axel)
    {
        switch (axel)
        {
            case "x":
                rb.AddRelativeForce(Vector3.right * force);
                break;
            case "y":
                rb.AddRelativeForce(Vector3.up * force);
                break;
            case "z":
                rb.AddRelativeForce(Vector3.forward * force);
                break;
        }
        
          
    //    rb.AddTorque(tf.up * force * 20f);
    
        }

    public void Wheel(Rigidbody rb,Transform tf, float force)
    {
        rb.AddTorque(tf.right * force);
    }


    private void Start()
    {
        
        AgentProto AgentProto = this.gameObject.GetComponent<AgentProto>();
        List<AgentProto.Component> components = AgentProto.GetComponents(); //All components of the robot
        AgentProto.Component LUpperArm = AgentProto.GetComponentByName("LUpperArm"); //One AgentParser.Component
        AgentProto.Component leftWheel = AgentProto.GetComponentByName("LWheelHitbox"); //One AgentParser.Component
        AgentProto.Component rightWheel = AgentProto.GetComponentByName("RWheelHitbox"); //One AgentParser.Component

        //AgentProto.Component obj = components[8];
        LUpperArm_RB = LUpperArm.gameObject.GetComponent<Rigidbody>(); //AgentParser.Component.gameObject contains the UnityEngine.GameObject
        LUpperArm_tf = LUpperArm.gameObject.GetComponent<Transform>();

        leftWheel_RB = leftWheel.gameObject.GetComponent<Rigidbody>(); //AgentParser.Component.gameObject contains the UnityEngine.GameObject
        leftWheel_tf = leftWheel.gameObject.GetComponent<Transform>();

        rightWheel_RB = rightWheel.gameObject.GetComponent<Rigidbody>(); //AgentParser.Component.gameObject contains the UnityEngine.GameObject
        rightWheel_tf = rightWheel.gameObject.GetComponent<Transform>();

        LUpperArm_RB.maxAngularVelocity = 1000f;
        Vector3 shoulderForce = new Vector3(leftShoulderX, leftShoulderY, leftShoulderZ);
        

    }

    public override List<float> CollectState()
	{
		List<float> state = new List<float>();

        state.Add(gameObject.transform.position.x);
        state.Add(gameObject.transform.position.y);
        state.Add(gameObject.transform.position.z);

        return state;
	}
    //This is called every frame.
    //Currently only works if brain is set to discrete. 
    public override void AgentStep(float[] act)
    {
    

        //move the wheels using controls from Movementcontrols script
        //Move.RotateWheel("left",act[0]);
        //Move.RotateWheel("right", act[1]);
        
        
        MoveArm(LUpperArm_RB, LUpperArm_tf, act[0], "x");
        MoveArm(LUpperArm_RB, LUpperArm_tf, act[1], "y");
        MoveArm(LUpperArm_RB, LUpperArm_tf, act[2], "z");

        Wheel(leftWheel_RB, leftWheel_tf, act[3]);
        Wheel(rightWheel_RB, rightWheel_tf, act[4]);

        reward =+ 1f;
        if(act[0] >9 || act[1] >9)
        {
            reward =+ 2f;
        }



    }

	public override void AgentReset()
	{

	}

	public override void AgentOnDone()
	{
       
    }
}
