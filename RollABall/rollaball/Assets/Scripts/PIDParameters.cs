/*
    Base class for pid parameters
    https://en.wikipedia.org/wiki/PID_controller
    
    These are used mainly with arm controls, but could be also used
    with more fine-tuned driving controls in the future
*/

using UnityEngine;
using System.Collections;

[System.Serializable]
public class PIDParameters
{
    // kp, ki and kd are tuning parameters
    // https://en.wikipedia.org/wiki/PID_controller#PID_controller_theory
    [Tooltip("Tuning parameter for current error. Increasing this value will make the current error having a larger effect on the output.")]
    public float kp;
    [Tooltip("Tuning parameter for accumulated error. Increasing this value will make the accumulated error having a larger effect on the output.")]
    public float ki;
    [Tooltip("Tuning parameter for error difference.. Increasing this value will make the error difference having a larger effect on the output.")]
    public float kd;
    [Tooltip("The maximum magnitude of the integrator. Larger values allow a larger accumulated error. Set this to nonzero to avoid the output from overshooting due to massive accumulated error.")]
    public float maxIntegratorMagnitude;
    [Tooltip("Don't edit, only visible for debugging purposes")]
    public float integrator;
    [Tooltip("Don't edit, only visible for debugging purposes")]
    public float currentError = 0;
    [Tooltip("Don't edit, only visible for debugging purposes")]
    public float prevError = 0;
    [Tooltip("Don't edit, only visible for debugging purposes")]
    public float output;

    public void AddError (float error)
    {
        prevError = currentError;
        currentError = error;
        // Scale integrator accumulated error by deltatime
        // Somewhat arbitrary but seems to keep the scale of the value
        // reasonable
        if (maxIntegratorMagnitude != 0f)
        {
            integrator = Mathf.Clamp(integrator + error*Time.deltaTime,
                                     -maxIntegratorMagnitude,
                                     maxIntegratorMagnitude);
        }
        else
        {
            integrator += error*Time.deltaTime;
        }
    }
    
    public float Output ()
    {
        output = currentError*kp + integrator*ki + (currentError - prevError)*kd;
        return output;
    }
    
    // Constructor
    public PIDParameters(float kp, float ki, float kd, float iMag = 0f)
    {
        kp = kp;
        ki = ki;
        kd = kd;
        maxIntegratorMagnitude = iMag;
    }
}
