// Low-level fucntions which control movement of the robot

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{

public class MovementControls : MonoBehaviour
{
    public Rigidbody body;
	public Battery battery;
	public WheelRotator leftWheel, rightWheel;
    public BallJoint[] ballJoints;
    public float slowingDistance;
    public PathFinding pathFinding;
    
    private float angleThreshold;
	private float torqueMod;
    private float distanceThreshold;
    private Vector3 waypoint, prevTarget;
    private bool getWaypoint;

	void Start () 
	{
		torqueMod = 1;
        angleThreshold = 3;
        distanceThreshold = 0.5f;
	}
    
    /*public int SetJointAngle(string jointName, Vector3 angle)
    {
        foreach (BallJoint bj in ballJoints)
        {
            if (bj.name == jointName)
            {
                int s = bj.SetAngle(angle);
                if (s != 0)
                {
                    battery.Deplete(0.1f);
                }
                return s;
            }
        }
        return -1;
    }*/
        
	
	public void RotateWheel(string wheel, float torque)
	// Ultimately, probably all moving components should consume battery straight in the baseclass
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
			leftWheel.Rotate(torque * torqueMod);
		}
		else
		{
			rightWheel.Rotate(torque * torqueMod);
		}

	}
	
	public void Turn(float force)
	// Turn left (negative force) or right (positive force)
	{
		float mod = 1; // Seems torque needs to be increased for turning, might need finetuning
		RotateWheel("left", force * mod);
		RotateWheel("right", -force * mod);
	}
	
	public void Drive(float force)
	// Drive backwards (negative force) or forwards (positive force)
	{
		RotateWheel("left", force);
		RotateWheel("right", force);
	}
	
	public int TurnTo(Vector3 target)
	// Turn towards a coordinate
	// Return 1 if angle to target is below threshold, -1 otherwise
	{
		float angle = Utils.AngleTo(body.position, body.transform.forward, target);
		if (Mathf.Abs(angle) > angleThreshold)
		{
			float direction = Mathf.Sign(angle);
			Turn(1 * direction);
			return -1;
		}
		else
		{
			return 1;
		}
	}
	public int TurnTo(Transform target)
	// Turn towards a transform
	// Return 1 if angle to target is below threshold, -1 otherwise
	{
		return TurnTo(target.position);
	}
	
	public int DriveTo(Vector3 target, bool pathfinding)
	// Drive near a position
	// Return 1 if distance to target is below threshold, -1 otherwise
	{
        // Check if we are already there
        var dist = new float();
		dist = Utils.FlatDist(body.position, target);
        
		if (Mathf.Abs(dist) <= distanceThreshold) {
			return 1;
		}
        
        // Get next waypoint if pathfinding
        if (pathfinding)
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
		dist = Utils.FlatDist(body.position, waypoint);
        Debug.Log(dist);
        
        // Current waypoint reached but not yet at target, get next one
        if (pathfinding && (Mathf.Abs(dist) <= distanceThreshold)) {
            getWaypoint = true;
            return -1;
        }
		if (TurnTo(waypoint) == -1) {
            //Debug.Log(string.Format("turning towards {0}", dest));
			return -1;
		}
        // Don't move too fast
        if (Mathf.Abs(dist) < slowingDistance * body.velocity.magnitude) {
			return -1;
		}
        // Did not reach destination yet but we are turned correctly,
        // decide if we should drive forwards or backwards
        var dir = (dist > distanceThreshold) ? 1 : -1;
		Drive(dir);
		return -1;

	}
    
	public int DriveTo(Vector3 target)
    {
        return DriveTo(target, false);
	}

    public int DriveTo(Transform target, bool pathfinding)
	// Drive near the position of a transform
	// Return 1 if distance to target is below threshold, -1 otherwise
	{
		return DriveTo(target.position);
	}
    
	public int DriveTo(Transform target)
	{
		return DriveTo(target.position, false);
	}
	
	void Update ()
	{
		//Debug.Log(Utils.Vec3FullAngle(body.right, new Vector3(goTo.x, 0, goTo.z)));
		//Debug.Log(Utils.AngleTo(body.position, body.right, goTo));
		//Debug.Log(battery.normLevel);
	}
}

} // End namespace