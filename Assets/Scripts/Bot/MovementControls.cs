/*
    Control movement of the robot
    
    Private functions in this script perform low level functions,
    such as rotating individual wheels,
    while pulic functions are intended for more general purpose usage,
    e.g. driving towards a target
    
    Check individual functions for usage
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{

public class MovementControls : MonoBehaviour
{
    [Tooltip("Modifier determining how early we should start slowing to avoid overshooting")]
    public float slowingDistanceModifier = 1;
    [Tooltip("Modifier for overall wheel rotation torque, fine-tune this to get a desired speed")]
	public float wheelTorqueModifier = 1f;
    [Tooltip("Additional modifier for wheel rotation torque during turning")]
	public float wheelTorqueTurningModifier = 2f;
    
    [Tooltip("Drag the rigidbody attached to the robot body here")]
    public Rigidbody robotBody;
    [Tooltip("Drag the battery attached to the robot here")]
	public Battery battery;
    [Tooltip("Drag the pathfinding script attached to the robot here")]
    public PathFinding pathFinding;
    [Tooltip("Drag the rigidbodies attached to robot wheels here")]
	public WheelRotator leftWheel, rightWheel;

    private bool getWaypoint = false;
    private Vector3 waypoint;
    private Vector3? prevTarget;  // The ? creates this as nullable, need
                                  // because first target might be 0,0,0 which
                                  // will mess up the pathfinding
	
	private void RotateWheel(string wheel, float torque)
    // Rotate the given wheel and deplete battery
	{
		if (battery.normLevel <= 0)
		{
			return;
		}
		if (torque != 0)
		{
			battery.Deplete(1);
		}
		if (wheel == "left")
		{
			leftWheel.Rotate(torque * wheelTorqueModifier);
		}
		else
		{
			rightWheel.Rotate(torque * wheelTorqueModifier);
		}

	}
	
	public void Turn(float force)
	// Turn left (negative force) or right (positive force)
	{
		RotateWheel("left", force * wheelTorqueTurningModifier);
		RotateWheel("right", -force * wheelTorqueTurningModifier);
	}
	
	public void Drive(float force)
	// Drive backwards (negative force) or forwards (positive force)
	{
		RotateWheel("left", force);
		RotateWheel("right", force);
	}
	
	public int TurnTo(Vector3 target, float angleThreshold=3.0f)
	/*
        Turn towards a target vector coordinate,
        until threshold in degrees is reached
        Return codes:
            0: Successfully turned towards target
            2: Turning in progress
    */
	{
		float angle = Utils.AngleTo(robotBody.position, robotBody.transform.forward, target);
		if (Mathf.Abs(angle) > angleThreshold)
		{
			float direction = Mathf.Sign(angle);
			Turn(1 * direction);
			return 2;
		}
		else
		{
			return 0;
		}
	}
	public int TurnTo(Transform target, float angleThreshold=3.0f)
	/*
        Turn towards a target coordinate
        overloaded function which gets the coordinates
        from the position of the target transform
    */
	{
		return TurnTo(target.position);
	}
	
	public int DriveTo(Vector3 target, bool enablePathfinding,
                       Collider other=null, float distanceThreshold=0.5f)
	/* 
        Drive near a position until distance threshold is reached.
        
        If collider is provided, then also return 0 if colliders are touching
        even before threshold is reached.
        
        Return codes:
            0: Target reached, distance below threshold
            2: Driving towards target otherwise
    */
	{
        // Check if we are already there
        var dist = new float();
		dist = Utils.FlatDist(robotBody.position, target);
        
		if (Mathf.Abs(dist) <= distanceThreshold) {
			return 0;
		}
        if (other != null && CheckCollision.Check(robotBody.GetComponent<Collider> (), other))
        {
            //Debug.Log("Touching other");
            return 0;
        }
        
        // Get next waypoint if pathfinding
        if (enablePathfinding)
        {
            if (target != prevTarget)
            {
                getWaypoint = true;
                pathFinding.SetDestination(target);
            }
            prevTarget = target;
            if (getWaypoint)
            {
                waypoint = pathFinding.NextWaypoint();
                getWaypoint = false;
            }
        }
        else
        {
            waypoint = target;
        }
        
        // Calculate distance again, needed because we might only be at the
        // waypoint and not the actual target
		dist = Utils.FlatDist(robotBody.position, waypoint);
        
        // Current waypoint reached but not yet at target, get next one
        if (enablePathfinding && (Mathf.Abs(dist) <= distanceThreshold)) {
            getWaypoint = true;
            return 2;
        }
		if (TurnTo(waypoint) == 2) {
            //Debug.Log(string.Format("turning towards {0}", dest));
			return 2;
		}
        // Don't move too fast
        if (Mathf.Abs(dist) < slowingDistanceModifier * robotBody.velocity.magnitude) {
			return 2;
		}
        // Did not reach destination yet but we are turned correctly,
        // decide if we should drive forwards or backwards
        var dir = (dist > distanceThreshold) ? 1 : -1;
		Drive(dir);
		return 2;

	}
    
    /*  
        Overloaded methods for driving, return codes as above
        
        Coordinates inferred from Transform, if Transform is used as target
        
        Collision detection is enabled if Transform is provided as the target
    */
	public int DriveTo(Vector3 target)
    {
        return DriveTo(target, false);
	}

    public int DriveTo(Transform target, bool enablePathfinding)
	{
        var coll = target.gameObject.GetComponent<Collider> ();
		return DriveTo(target.position, enablePathfinding, coll);
	}
    
	public int DriveTo(Transform target)
	{
		return DriveTo(target, false);
	}
        //api2017 
        //Motor controls
       
    public void Joint(Rigidbody rb, Transform tf, float force, int axel)
        {
            switch (axel)
            {


                case 0:
                    rb.AddRelativeTorque(Vector3.right * force);
                    break;
                case 1:
                    rb.AddRelativeTorque(Vector3.up * force);
                    break;
                case 2:
                    rb.AddRelativeTorque(Vector3.forward * force);
                    break;


            }

        }

        public void Wheel(Rigidbody rb, Transform tf, float force,int axel)
        {
            switch (axel)
            {


                case 0:
                    rb.AddTorque(Vector3.right * force);
                    break;
                case 1:
                    rb.AddTorque(Vector3.up * force);
                    break;
                case 2:
                    rb.AddTorque(Vector3.forward * force);
                    break;


            }
        }

        

        //
    }

} // End namespace