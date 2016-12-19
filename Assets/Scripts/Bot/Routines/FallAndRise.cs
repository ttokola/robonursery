/*
    Fall deliberately, and get back up using hands
    Rising up is not physically accurate
    
    Routine is implemented with a state-machine, do not attempt to move
    the bot while routine is in progress
*/
    
using UnityEngine;
using System.Collections;

namespace LilBotNamespace
{
    
public class FallAndRise : MonoBehaviour
{
    public Rigidbody body;
    
    public ArmControls armControls;
    public Rigidbody[] wheels;

    private float fallAndRiseStartTime;
    private float fallenTime;
    private float riseupStart;
    private int fallAndRiseState = 0;
    private RigidbodyConstraints oldConstraints;
    
    public int Execute ()
    /*
        Call this continuously to execute the routine
        Return codes:
        0: Routine complete
        2: Routine in progress
    */
    {
        // State-machine for the fall and rise routine
        switch (fallAndRiseState)
        {
        // Routine start
        case 0:
            oldConstraints = body.constraints;
            body.constraints = RigidbodyConstraints.None;
            fallAndRiseStartTime = Time.time;
            fallAndRiseState = 1;
            break;
        // No rotation restriction, falling down
        case 1:
            if (Mathf.Abs(Utils.AngleNorm180(body.transform.eulerAngles.x)) > 45f)
            {
                fallAndRiseState = 2;
                fallenTime = Time.time;
            }
            break;
        // I've fallen and can't get up (angle above threshold), wait a few seconds to stabilize bot
        case 2:
            if ((Time.time - fallenTime) > 5f)
            {
                fallAndRiseState = 3;
            }
            break;
        // First move arms to the sides (just in case they are in a weird position), then forward or backward depending on which side we fell
        case 3:
            if (armControls.SetStaticPosition("sides") == 0)
            {
                fallAndRiseState = 4;
            }
            break;
        // Try to rise up using arms, lock wheels to prevent sliding
        case 4:
            foreach (Rigidbody rb in wheels)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
            string pos = (Utils.AngleNorm180(body.transform.eulerAngles.x) > 0) ? "forward" : "back";
            armControls.SetStaticPosition(pos);
            if (Mathf.Abs(Utils.AngleNorm180(body.transform.eulerAngles.x)) < 30f)
            {
                fallAndRiseState = 5;
                riseupStart = Time.time;
            }
            break;
        // Move back upright
        case 5:
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
            body.constraints = oldConstraints;
            body.MoveRotation(Quaternion.Euler(Vector3.zero));
            if (Time.time - riseupStart > 5f) // Assume we are up after 5s
            {
                foreach (Rigidbody rb in wheels)
                {
                    rb.constraints = RigidbodyConstraints.None;
                }
                fallAndRiseState = 0; // Go back to beginning
                return 0;
            }
            break;            
        }
        
        return 2;
    }
}

} // End namespace