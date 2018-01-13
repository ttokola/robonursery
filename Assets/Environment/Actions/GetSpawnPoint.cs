/*
Script to get clear spawn point

This script is attached to transform object and then it can be called to get valid spawn point inside defined radius
Example call: Vector3 p = GameObject.Find("Spawn_Livingroom").GetComponent<GetSpawnPoint>().getSpawnPoint();
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSpawnPoint : MonoBehaviour {
    public float spawnAreaRadius = 5.0f;

    public float checkAreaRadius = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 getSpawnPoint()
    /*
     Get valid spawnpoint for this transformation
     
         
     On success returns Vector3 of valid non-collision spawnpoint 
     On failure returns Vector3.negativeInfinity
     */
    {
        float rand1 = 0;
        float rand2 = 0;
        int i = 0;

        while (true)
        {
            rand1 = (Random.value - 0.5f) * 2;
            rand2 = (Random.value - 0.5f) * 2;
            Vector3 rand = new Vector3(spawnAreaRadius * rand1,0, spawnAreaRadius * rand2);
            Vector3 pos = transform.position + rand;


            Vector3 boxOffset = new Vector3(0, 1.1f, 0);
            Vector3 boxHalfDimension = new Vector3(checkAreaRadius, 0.5f, checkAreaRadius);

            Collider[] hitColliders = Physics.OverlapBox(pos + boxOffset, boxHalfDimension);

            if (hitColliders.Length == 0)
            {
                // Success
                return pos;
            }
            i++;

            if (i > 5)
            {
                // Failure
                return Vector3.negativeInfinity;
            }
        }
    }
}
