/*
    Add simple collision check script to object, used with
    main collision check script (CheckCollision.cs)
*/

using UnityEngine;
using System.Collections;

public class CollisionWrapper : MonoBehaviour
{
    public bool touching = false;
    
    public Collider otherCollider;
    
	void OnCollisionEnter(Collision other)
    {
        if (other.collider == otherCollider)
        {
            touching = true;
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider == otherCollider)
        {
            touching = false;
        }
    }
}
