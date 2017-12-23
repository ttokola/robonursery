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
                Debug.Log("onnistui");
                //Debug.Log(pos.ToString());
                return pos;
            }
            else
            {
                //Debug.Log("kierrosepäonnistui");
            }
            i++;

            if (i > 5)
            {
                //Debug.Log("kokonaanepäonnistui");
                return transform.position;
            }
        }
    }
}
