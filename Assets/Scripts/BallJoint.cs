using UnityEngine;
using System.Collections;

public class BallJoint : MonoBehaviour {
    
    public Rigidbody master;
    public Rigidbody slave;
    
	void Start ()
    {
        Debug.Log(master);
    }
}