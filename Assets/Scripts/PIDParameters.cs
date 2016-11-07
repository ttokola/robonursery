using UnityEngine;
using System.Collections;

[System.Serializable]
public class PIDParameters
{
    // kp, ki and kd are tuning parameters
    // https://en.wikipedia.org/wiki/PID_controller#PID_controller_theory
    [Tooltip("Tuning parameter for current error")]
    public float kp;
    [Tooltip("Tuning parameter for accumulated error")]
    public float ki;
    [Tooltip("Tuning parameter for error difference")]
    public float kd;
    [Tooltip("The maximum magnitude of the integrator, this should be nonzero to avoid overshooting of the output due to high accumulator value")]
    public float maxIntegratorMagnitude;
    [Tooltip("Don't edit, only visible for debugging purposes")]
    public float integrator;
    [Tooltip("Don't edit, only visible for debugging purposes")]
    public float currentError = 0;
    [Tooltip("Don't edit, only visible for debugging purposes")]
    public float prevError = 0;

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
        return currentError*kp + integrator*ki + (currentError - prevError)*kd;
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
