using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class MLAagent : Agent {


    private Vector3 shoulderForce;

    private Rigidbody LUpperArm_RB;

    private Transform LUpperArm_tf;

    private Rigidbody leftWheel_RB;
    private Transform leftWheel_tf;

    private Rigidbody rightWheel_RB;
    private Transform rightWheel_tf;

    private MovementControls motor;



    private void Start()
    {
       motor = this.gameObject.GetComponent<MovementControls>();


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
        
        
        motor.MoveArm(LUpperArm_RB, LUpperArm_tf, act[0], "x");
        motor.MoveArm(LUpperArm_RB, LUpperArm_tf, act[1], "y");
        motor.MoveArm(LUpperArm_RB, LUpperArm_tf, act[2], "z");

        motor.Wheel(leftWheel_RB, leftWheel_tf, act[3]);
        motor.Wheel(rightWheel_RB, rightWheel_tf, act[4]);

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
