/*
    Configures a ball joint.
    
    The object should be oriented so that y points up and z points forward. The
    object origin should be set to the center of the imaginary ball joint.
*/

using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PIDParams
{
    public float kp;
    public float ki;
    public float kd;
    public Vector3 integrator;
    public Vector3 prevError;
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
        RotateRbToAngle(horRb, new Vector3(0, horAngle, 0), maxHorizontalForce, ref horPid);
        RotateRbToAngle(verRb, new Vector3(0, 0, verAngle), maxVerticalForce, ref verPid);
        if (Time.time - start > 5)
        {
             //joint.angularZMotion = ConfigurableJointMotion.Limited;
             //Debug.Log("Locked");
             //RotateToAngle(new Vector3(0, 45, 0));
        }
    }
    
    float Clamp180 (float angle)
    {
        angle = angle % 360;
        if (angle > 180)
        {
            return -(360 - angle);
        }
        else if (angle < -180)
        {
            return 360 + angle;
        }
        return angle;
    }
    
    Vector3 Clamp180 (Vector3 angle)
    {
        angle.x = Clamp180(angle.x);
        angle.y = Clamp180(angle.y);
        angle.z = Clamp180(angle.z);
        return angle;
    }
    
    int RotateRbToAngle (Rigidbody rb, Vector3 angle, float maxTorque, ref PIDParams pp)
    // Attempt to rotate a rigidbody to rotation by adding torque
    {   // Adding torque towards angle that can't be reached could rotate the parents
        var damping = 10f;
        rb.angularDrag = damping;
        var minDamp = damping;
        var maxDamp = 50f;
        var errorThreshold = 5f;
        var localRot = rb.rotation.eulerAngles;
        angle = connected.rotation.eulerAngles + angle; // Rotate relative to parent
        angle = Clamp180(angle);
        localRot = Clamp180(localRot);
        var error = angle - localRot;
        
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
        
        var errorDiff = error - pp.prevError;
        pp.integrator += error * Time.deltaTime;
        var torque = Vector3.ClampMagnitude((error * pp.kp + pp.integrator * pp.ki + errorDiff * pp.kd), maxTorque);
        Debug.Log(string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", rb, rb.rotation.eulerAngles, localRot, angle, error, errorDiff, torque, pp.integrator, pp.prevError));
        rb.AddRelativeTorque(torque);
        pp.prevError = error;
        
        return 0;
    }   
}