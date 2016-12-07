using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour
{

    public GameObject botPrefab;
    public string botName;
    //public int index;
    ScenarioScript scenario;


    // Use this for initialization
    void Start()
    {
        scenario = transform.parent.GetComponent<ScenarioScript>();
        Vector3 pos = transform.position;
        GameObject bot = (GameObject)Instantiate(botPrefab, pos, Quaternion.identity);
        bot.name = botName;
        scenario.addBot(bot);
        //scenario.botList.Add(bot);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
