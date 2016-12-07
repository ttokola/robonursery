using UnityEngine;
using System.Collections;

public class JointMotorScript : MonoBehaviour {

    public string axisname = "";
    public float max_force = 100.0f;
    public float max_velocity = 300.0f;
    public bool damping = true;
    private float angle_t0;
    private float angle_t1;
    private bool target_set_t0;
    private bool target_set_t1;
    private HingeJoint hinge;
    private JointMotor motor;

    private float velocity;

    void Start () {
        // init motor
        hinge = GetComponent<HingeJoint>();
        motor = hinge.motor;
        motor.force = max_force;
        motor.targetVelocity = 0;
        motor.freeSpin = false;
        hinge.motor = motor;
        hinge.useMotor = true;

        // init damping control
        angle_t0 = hinge.angle;
        angle_t1 = hinge.angle;
        target_set_t0 = false;
        target_set_t1 = false;
        
    }

	
	void FixedUpdate ()
    {
        motor = hinge.motor;

        velocity = Input.GetAxis(axisname) * max_velocity;
        if (Mathf.Abs(velocity) > 1.0f)
        {
            target_set_t1 = true;
        }
        else
        {
            target_set_t1 = false;
        }

        if (damping == true)
        {
            damp();
        }

        motor.targetVelocity = velocity;
        hinge.motor = motor;
        
    }

    void damp()
    {
        float old_target = motor.targetVelocity;
        angle_t1 = hinge.angle;

        // prevent flooding down when new input and old input is ~ zero
        if (target_set_t0 == false && target_set_t1 == false)
        {
            float delta_angle_per_sec = (angle_t0 - angle_t1) / Time.deltaTime;
            velocity = delta_angle_per_sec + old_target;

            //Debug.Log(delta_angle_per_sec);

            if(velocity > max_velocity)
            {
                velocity = 0.0f;
            }
        }

        angle_t0 = angle_t1;
        target_set_t0 = target_set_t1;
    }

}
