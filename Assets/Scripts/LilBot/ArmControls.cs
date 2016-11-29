using UnityEngine;
using System.Collections;

public class ArmControls : MonoBehaviour
{
    
    public BallJoint leftUpper;
    public BallJoint leftLower;
    public BallJoint rightUpper;
    public BallJoint rightLower;
    
    private string prevPosition;
    
    private int waveState = 0;
    
    public int SetStaticPosition (string position)
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
        case "wave1":
            lAngles = new float[] {0, 0, 0, -80};
            rAngles = new float[] {-90, -30, -90, 30};
            break;
        case "wave2":
            lAngles = new float[] {0, 0, 0, -110};
            rAngles = new float[] {-90, -30, -90, 30};
            break;
        }
        // Ugly
        var a = leftUpper.SetAngle(lAngles[0], lAngles[1]);
        var b = leftLower.SetAngle(lAngles[2], lAngles[3]);
        var c = rightUpper.SetAngle(rAngles[0], rAngles[1]);
        var d = rightLower.SetAngle(rAngles[2], rAngles[3]);
        
        // Reset pid params if we are adjusting to new position
        /*if (position != prevPosition)
        {
            Debug.Log("here");
            leftUpper.ResetPID();
            leftLower.ResetPID();
            rightUpper.ResetPID();
            rightLower.ResetPID();
        }*/
        
        prevPosition = position;
        
        // Ugly
        if (a != 0 || b != 0 || c != 0 || d != 0)
        {
            return -1;
        }
        return 0;
    }
    
    public void Wave ()
    {
        if (waveState == 0 && SetStaticPosition("wave1") == 0)
        {
            waveState = 1;
        }
        if (waveState == 1 && SetStaticPosition("wave2") == 0)
        {
            waveState = 0;
        }
    }
    
    public void Dance ()
    {
        ;
    }
}
