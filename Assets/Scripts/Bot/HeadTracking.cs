/*
    Track the head towards a specified transform
*/

using UnityEngine;
using System.Collections;

public class HeadTracking : MonoBehaviour {

    public bool trackingEnabled = true;
    public float xLimit = 90;
    public float yLimit = 45;
    public float headMovementSpeedMod = 1f;
    
    public Rigidbody head;
    public Rigidbody neck;
    [Tooltip("The target transform towards which the head will be rotated, if possible")]
    public Transform target;
    
    private float timer;
    
    private Quaternion targetRot, prevTargetRot;
    
    void Start ()
    {
        timer = Time.time;
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
            if (Mathf.Abs(diff.x) > xLimit || Mathf.Abs(diff.y) > yLimit)
            {
                targetRot = neck.rotation;
            }
        }
        else
        {
            targetRot = neck.rotation;
        }
        // Move head smoothly over time
        if (targetRot != prevTargetRot)
        {
            timer = Time.time;
        }
        var lerpedRotation = Quaternion.Lerp(
            head.rotation, targetRot, (Time.time-timer) * headMovementSpeedMod
        );
        head.MoveRotation(lerpedRotation);
        
        prevTargetRot = targetRot;
	}
}
