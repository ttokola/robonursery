using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleControl : MonoBehaviour {

    public void MoveArm(Rigidbody obj, Vector3 force)
    {

        obj.AddTorque(force);
    }

}
