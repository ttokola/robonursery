/*
    Configures a ball joint.
    
    The object should be oriented so that y points up and z points forward. The
    object origin should be set to the center of the imaginary ball joint.
*/

using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PIDParams
// kp, ki and kd are tuning parameters
// https://en.wikipedia.org/wiki/PID_controller#PID_controller_theory
{
    public float kp;
    public float ki;
    public float kd;
    public float integrator;
    public float prevError;
}

public class BallJoint : MonoBehaviour {

    public float maxHorizontalForce, maxVerticalForce;
    public float horAngle, verAngle;
    public float angleLimit;
    public Rigidbody connected;
    public PIDParams horPid, verPid;
    
    private float start;
    private ConfigurableJoint horJoint, verJoint;
    private GameObject horJointContainer;
    private Rigidbody horRb, verRb; // horRb is the invisible rigidbody which holds the horizontal joint, verRb is this rigidbody
    
	void Start ()
    {
        verRb = GetComponent<Rigidbody> ();
        // We need two joints, set up an invisible gameobject with rigidbody to which this object and the second joint connects
        horJointContainer = new GameObject("");
        horJointContainer.transform.position = GetComponent<Transform> ().position;
        horRb = horJointContainer.AddComponent<Rigidbody> ();
        //horRb.useGravity = false;
        // Need to move fast enough
        horRb.maxAngularVelocity = verRb.maxAngularVelocity = 100;
        // Set up the configurable joints for the fake and main rigidbody
        // The fake rigidbody rotates in vertically around z-axis and the main rigidbody horizontally around y-axis
        var jointLimit = new SoftJointLimit();
        jointLimit.limit = angleLimit;
        
        horJoint = horJointContainer.AddComponent<ConfigurableJoint> ();
        horJoint.anchor = Vector3.zero;
        horJoint.connectedBody = connected;
        horJoint.xMotion = horJoint.yMotion = horJoint.zMotion = ConfigurableJointMotion.Locked; // We only want angular motion
        horJoint.angularXMotion = ConfigurableJointMotion.Locked;
        horJoint.angularYMotion = ConfigurableJointMotion.Limited;
        horJoint.angularZMotion = ConfigurableJointMotion.Locked;
        horJoint.angularYLimit = jointLimit;
        
        verJoint = gameObject.AddComponent<ConfigurableJoint> ();
        verJoint.anchor = Vector3.zero;
        verJoint.connectedBody = horRb;
        verJoint.xMotion = verJoint.yMotion = verJoint.zMotion = ConfigurableJointMotion.Locked; // We only want angular motion
        verJoint.angularXMotion = ConfigurableJointMotion.Locked;
        verJoint.angularYMotion = ConfigurableJointMotion.Locked;
        verJoint.angularZMotion = ConfigurableJointMotion.Limited;
        verJoint.angularZLimit = jointLimit;
        
        start = Time.time;
    }
    
    void FixedUpdate ()
    {
        RotateRbToAngle(horRb, "y", horAngle, maxHorizontalForce, ref horPid);
        RotateRbToAngle(verRb, "z", verAngle, maxVerticalForce, ref verPid);
        if (Time.time - start > 5)
        {
             //joint.angularZMotion = ConfigurableJointMotion.Limited;
             //Debug.Log("Locked");
             //RotateToAngle(new Vector3(0, 45, 0));
        }
    }
    
    int RotateRbToAngle (Rigidbody rb, string axle, float angle, float maxTorque, ref PIDParams pp)
    // Attempt to rotate a rigidbody to rotation by adding torque
    {
        var damping = 10f;
        rb.angularDrag = damping;
        //var minDamp = damping;
        //var maxDamp = 50f;
        //var errorThreshold = 5f;
        var localRot = new float();
        switch (axle)
        {
        case "y":
            localRot = rb.rotation.eulerAngles.y;
            angle = connected.rotation.eulerAngles.y + angle; // Rotate relative to parent
            break;
        case "z":
            localRot = rb.rotation.eulerAngles.z;
            angle = connected.rotation.eulerAngles.z + angle; // Rotate relative to parent
            break;
        default:
            Debug.LogError(string.Format("Unknown axle {0}", axle));
            return -1;
        }
        var error = Utils.AngleDiff180(localRot, angle);
        // Break and apply damping if error below threshold
        /*if (Mathf.Abs(error) < errorThreshold)
        {
            //Debug.Log("here");
            var dampingMul = (errorThreshold - Mathf.Abs(error))/errorThreshold;  // Returns 1 when error is lowest
            //damping = Mathf.Max(minDamp, Mathf.Pow(dampingMul, 1) * maxDamp);
            damping = maxDamp;
            rb.angularDrag = damping;
            pp.integrator = 0;
            rb.angularVelocity = Vector3.zero;
            return 0;
        }*/
        
        pp.integrator += error * Time.deltaTime;
        var errorDiff = error - pp.prevError;
        var temp = error*pp.kp + pp.integrator*pp.ki + errorDiff*pp.kd;
        var torque = new Vector3();
        switch (axle)
        {
        case "y":
            torque = new Vector3(0, temp, 0);
            break;
        case "z":
            torque = new Vector3(0, 0, temp);
            break;
        default:
            Debug.LogError(string.Format("Unknown axle {0}", axle));
            return -1;
        }
        torque = Vector3.ClampMagnitude(torque, maxTorque);
        //Debug.Log(string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", rb, rb.rotation.eulerAngles, localRot, angle, error, errorDiff, torque, pp.integrator, pp.prevError));
        rb.AddRelativeTorque(torque);
        pp.prevError = error;
        
        return 0;
    }   
}