using UnityEngine;
using System.Collections;

public class ScenarioScript : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        
        Instantiate(prefab, new Vector3(0.0f, 3.0f, 0.0f), Quaternion.identity);
    }

// Update is called once per frame
void Update () {
	
	}
}
