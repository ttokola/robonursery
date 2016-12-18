/*
    Ignore collision between sets of colliders
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Why do we need a wrapper? answer:
// http://answers.unity3d.com/questions/289692/serialize-nested-lists.html
[System.Serializable]
public class ListWrapper
{
    public List<Collider> colliders;
}

public class IgnoreCollision : MonoBehaviour {

    public List<ListWrapper> ignoredColliderSets;
    public List<Collider> ignoreAllCollisions;
    
	void Start ()
    {
        foreach (ListWrapper lw in ignoredColliderSets)
        {
            // These loops just create all the possible combinations from the collider set
            // A bit ugly but couldn't find native solution for list combinations
            for (int i = 0; i < lw.colliders.Count; i++)
            {
                for (int j = i+1; j < lw.colliders.Count; j++)
                {
                    Physics.IgnoreCollision(lw.colliders[i], lw.colliders[j]);
                }
            }
        }
        foreach (Collider a in ignoreAllCollisions)
        {
            foreach (Collider b in Object.FindObjectsOfType<Collider> ())
            {
                Physics.IgnoreCollision(a, b);
            }
        }
    }
}
