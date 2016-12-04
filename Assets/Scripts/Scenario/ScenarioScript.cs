using UnityEngine;
using System.Collections;

public class ScenarioScript : MonoBehaviour
{
    public GameObject prefab;
    public GameObject tar;
    LilBotNamespace.PickupObject picking;
    //public 

    void Start()
    {
        
        GameObject bot = (GameObject) Instantiate(prefab, new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);

        

        picking = bot.GetComponent<LilBotNamespace.PickupObject>();
        picking.tar = tar;

        picking.enabled = true;



        bool value = bot.GetComponent<Battery>().isActiveAndEnabled;

        bool value2 = bot.GetComponent < LilBotNamespace.MovementControls>();

        Debug.Log(value);
        Debug.Log(value2);
    }

// Update is called once per frame
void Update () {
	
	}
}
