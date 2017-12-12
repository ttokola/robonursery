/*
    Wrapper script to check if two objects are touching
*/

using UnityEngine;
using System.Collections;

public class CheckCollision : MonoBehaviour
{
    private static CollisionWrapper wrapper;
    
    public static bool Check (Collider coll1, Collider coll2, bool checkObjectTree=true)
    /*
        Check if two objects are touching
        
        If checkObjectTree is true, then check if any object related object
        in first param is touching with any object related to
        object in second param
        
        Returns true if touching, false otherwise
    */
    {
        if (!checkObjectTree)
        {
            wrapper = AddWrapper(coll1, coll2);
            return wrapper.touching;
        }
        foreach (Collider c1 in coll1.transform.root.GetComponentsInChildren<Collider> ())
        {
            foreach (Collider c2 in coll2.transform.root.GetComponentsInChildren<Collider> ())
            {
                wrapper = AddWrapper (c1, c2);
                if (wrapper.touching)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    // Convinience function to add collision wrapper to other object
    static CollisionWrapper AddWrapper (Collider coll1, Collider coll2)
    {
        var wrapper = coll1.gameObject.GetComponent<CollisionWrapper> ();
        if (wrapper == null)
        {
            wrapper = coll1.gameObject.AddComponent<CollisionWrapper> ();
        }
        wrapper.otherCollider = coll2;
        return wrapper;
    }
}