/*
    Control facial expressions and eyebrow angle
    
    To add a new expression, create a new texture,
    create a new enumeration for the expression,
    and map the texture to the enumeration in SetMouth(MouthState state)
    
    Also map a string to this enumeration SetMouth(string state)
*/

using UnityEngine;
using System.Collections;

public class FaceControls : MonoBehaviour {
    
    // Enumeration for the different mouht states
    public enum MouthState {smiling, neutral, frowning};
    
    public Renderer mouthRend;
    public Transform leftEyebrow, rightEyebrow;
    public Texture smilingTexture, neutralTexture, frowningTexture;
    
    public MouthState mouth;
    public float eyebrowAngle = 0f;
    
    private Material mouthMat;
    
	void Start ()
    {
        mouthMat = Instantiate(mouthRend.material);
	}
	
    /*
        Mapping of strings to mouth state enum
        We want to map strings to enums so we can set the facial expressions
        From the inspector too, using the enum, but can use a string to set
        The expression from another function
    */
	public void SetMouth (string state)
    // Use this function to set the mouth expression from other scripts
    {
        switch (state)
        {
        case "smiling":
            mouth = MouthState.smiling;
            break;
        case "neutral":
            mouth = MouthState.neutral;
            break;
        case "frowning":
            mouth = MouthState.frowning;
            break;
        }
	}
    
    void SetMouth (MouthState state)
    // Set the mouth expression using enum, don't use this outside this script
    {
        switch (state)
        {
        case MouthState.smiling:
            mouthMat.SetTexture("_MainTex", smilingTexture);
            break;
        case MouthState.neutral:
            mouthMat.SetTexture("_MainTex", neutralTexture);
            break;
        case MouthState.frowning:
            mouthMat.SetTexture("_MainTex", frowningTexture);
            break;
        }
        mouthRend.material = mouthMat;
    }
    
    public void SetEyebrowAngle (float angle)
    // Set the eyebrows to an arbitrary angle
    {
        eyebrowAngle = angle;
    }

    void SetEyebrowAngle (string expr)
    // Set the eyebrows to a predetermined angle, specified by a string
    {
        switch (expr)
        {
        case "neutral":
            eyebrowAngle = 0f;
            break;
        case "angry":
            eyebrowAngle = 20f;
            break;
        case "submissive":
            eyebrowAngle = -20f;
            break;
        }
    }

    void RotateEyebrows ()
    // Rotate the eyebrows, called on Update()
    {
        var rot = leftEyebrow.rotation.eulerAngles;
        rot.z = -eyebrowAngle;
        leftEyebrow.transform.rotation = Quaternion.Euler(rot);
        rot = rightEyebrow.rotation.eulerAngles;
        rot.z = eyebrowAngle;
        rightEyebrow.transform.rotation = Quaternion.Euler(rot);
    }
    
    void Update ()
    {
        // Set the facial expression on every update
        SetMouth(mouth);
        RotateEyebrows();
    }
}
