using UnityEngine;
using System.Collections;

public class FaceControls : MonoBehaviour {
    
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
	
    // Mapping of strings to mouth state enum
	void SetMouth (string state)
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
    
    void SetEyebrowAngle (float angle)
    {
        var rot = leftEyebrow.rotation.eulerAngles;
        rot.z = -angle;
        leftEyebrow.transform.rotation = Quaternion.Euler(rot);
        rot = rightEyebrow.rotation.eulerAngles;
        rot.z = angle;
        rightEyebrow.transform.rotation = Quaternion.Euler(rot);
    }
    void SetEyebrowAngle (string expr)
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
    
    void Update ()
    {
        SetMouth(mouth);
        SetEyebrowAngle(eyebrowAngle);
    }
}
