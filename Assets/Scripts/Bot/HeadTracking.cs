/*
    Track the head towards a specified transform
*/

using UnityEngine;
using System.Collections;

public class HeadTracking : MonoBehaviour {

    [Tooltip("Enable actual tracking, keep the script always enabled to avoid head flopping around")]
    public bool trackingEnabled = true;
    [Tooltip("Maximum vertical angle of the head")]
    public float maxVerticalAngle = 45;
    [Tooltip("Maximum horizontal angle of the head")]
    public float maxHorizontalAngle = 90;
    [Tooltip("Modifier for the rotation speed of the head")]
    public float headMovementSpeedMod = 100f;
    
    public ConfigurableJoint neckJoint;
    [Tooltip("Drag the rigidbody attached to the robot head here")]
    public Rigidbody head;
    [Tooltip("Drag the rigidbody attached to the robot neck here. Neck is considered as the neutral position to which the head rotation is compared.")]
    public Rigidbody neck;
    [Tooltip("The target transform towards which the head will be rotated, if possible. Change this from other scripts to change the target dynamically.")]
    public Transform target;
    
    private Quaternion targetRot, prevTargetRot;
    
    void Start ()
    {
        var lim = new SoftJointLimit();
        lim.limit = maxVerticalAngle;
        neckJoint.lowAngularXLimit = neckJoint.highAngularXLimit = lim;
        lim.limit = maxHorizontalAngle;
        neckJoint.angularYLimit = lim;
    }
	
	void FixedUpdate ()
    {
        if (trackingEnabled && target != null)
        {
            var dir = (target.position - head.transform.position);
            targetRot = Quaternion.LookRotation(dir);
            var diff = Utils.AngleDiff180(targetRot.eulerAngles,
                                          neck.rotation.eulerAngles);
            // Do not rotate over limits
            if (Mathf.Abs(diff.x) > maxVerticalAngle || Mathf.Abs(diff.y) > maxHorizontalAngle)
            {
                targetRot = neck.rotation;
            }
        }
        else
        {
            targetRot = neck.rotation;
        }
        var lerpedRotation = Quaternion.Lerp(
            head.rotation, targetRot, Time.deltaTime * headMovementSpeedMod
        );
        head.MoveRotation(lerpedRotation);
        
        prevTargetRot = targetRot;
	}
}
