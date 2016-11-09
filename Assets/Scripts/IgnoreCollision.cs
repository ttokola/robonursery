/*
    Ignore collision between collider pairs
*/

using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ColliderPair
{
    public Collider coll1;
    public Collider coll2;
}

public class IgnoreCollision : MonoBehaviour {

    public ColliderPair[] colliderPairs;
    
	void Start ()
    {
        foreach (ColliderPair cp in colliderPairs)
        {
            Physics.IgnoreCollision(cp.coll1, cp.coll2);
        }
	}
}
