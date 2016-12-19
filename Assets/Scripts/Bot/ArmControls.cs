/*
    Control the arms using ball joints
*/

using UnityEngine;
using System.Collections;

public class ArmControls : MonoBehaviour
{
    [Tooltip("Drag the balljoint script attached to the upper left arm here")]
    public BallJoint leftUpper;
    [Tooltip("Drag the balljoint script attached to the lower left arm here")]
    public BallJoint leftLower;
    [Tooltip("Drag the balljoint script attached to the upper right arm here")]
    public BallJoint rightUpper;
    [Tooltip("Drag the balljoint script attached to the lower right arm here")]
    public BallJoint rightLower;
    
    public int SetStaticPosition (float[] leftAngles, float[] rightAngles)
    /*
        Set the arms into any static angle
        Both leftAngles and rightAngles should have 4 values, one for
        upper X, upper Y, lower X and lower Y angles
    */
    {
        var leftUpperStatus = leftUpper.SetAngle(leftAngles[0], leftAngles[1]);
        var leftLowerStatus = leftLower.SetAngle(leftAngles[2], leftAngles[3]);
        var rightUpperStatus = rightUpper.SetAngle(rightAngles[0], rightAngles[1]);
        var rightLowerStatus = rightLower.SetAngle(rightAngles[2], rightAngles[3]);
        int[] statuses = {leftUpperStatus, leftLowerStatus,
                          rightUpperStatus, rightLowerStatus};
        // Check for bad return codes
        foreach (var s in statuses)
        {
            if (s == 1)
            {
                return 1;
            }
        }
        foreach (var s in statuses)
        {
            if (s != 0)
            {
                return s;
            }
        }
        return 0;
    }
    
    public int SetStaticPosition (string position)
    /*
        Set the arms into a predetermined static position
        Available position strings and their correspoding arm positions:
            idle: folded in front of the robot
            sides: extended to the sides
            forward: extended forwards
            forwardL: extended forwards, leaning down
            forwardH: extended forwards, leaning up
            back: extended backwards
            down: extended down
            up: extended up
            
        New static positions can be added by adding correspoding
        string-angle mapping to the switch.
    */    
    {
        var lAngles = new float[4];
        var rAngles = new float[4];
        switch (position)
        {
        case "idle":
            lAngles = new float[] {90, 10, 90, 0};
            rAngles = new float[] {-90, -30, -90, 0};
            break;
        case "sides":
            lAngles = new float[] {0, 0, 0, 0};
            rAngles = new float[] {0, 0, 0, 0};
            break;
        case "forward":
            lAngles = new float[] {90, 0, 0, 0};
            rAngles = new float[] {-90, 0, 0, 0};
            break;
        case "forwardL":
            lAngles = new float[] {90, 40, 0, 0};
            rAngles = new float[] {-90, -40, 0, 0};
            break;
        case "forwardH":
            lAngles = new float[] {90, -40, 0, 0};
            rAngles = new float[] {-90, 40, 0, 0};
            break;
        case "back":
            lAngles = new float[] {-90, 0, 0, 0};
            rAngles = new float[] {90, 0, 0, 0};
            break;
        case "down":
            lAngles = new float[] {0, 70, 0, 20};
            rAngles = new float[] {0, -70, 0, -20};
            break;
        case "up":
            lAngles = new float[] {0, -90, 0, 0};
            rAngles = new float[] {0, 90, 0, 0};
            break;
        }
        return SetStaticPosition(lAngles, rAngles);
        
        // Pid resetting might actually cause wrong movements, don't do this
        // Saved for future reference
        /*
        // Reset pid params if we are adjusting to new position
        if (position != prevPosition)
        {
            Debug.Log("here");
            leftUpper.ResetPID();
            leftLower.ResetPID();
            rightUpper.ResetPID();
            rightLower.ResetPID();
        }*/
    }
}
