using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agentParser : MonoBehaviour {

    // Use this for initialization
    public bool RigidBody = true;
    [Header("Agent parameters")]

        public int ActionSize;
        public int StateSize;

        public GameObject[] InUse;

    [System.Serializable]
    public struct Component { 
        public GameObject gameObject;
        public enum Type
        {
            Joint,
            Wheel
        }
        public int Axes;
        public Vector3 Dimensions;
        public int[] InputIndeces;

    }
    public Component LForeArm;
    public Component LUpperArm;
    public Component RForeArm;
    public Component RUpperArm;
   
}
