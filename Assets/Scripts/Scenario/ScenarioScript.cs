using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenarioScript : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> botList;
    [HideInInspector]
    public Dictionary<string, int> nameDict;
        
    //public SortedList positions;


    public ScenarioScript()
   {
        botList = new List<GameObject>();
        nameDict = new Dictionary<string, int>();
    }

    void Start()
    {
        
        /*
        bool value = botList[0].GetComponent<Battery>().isActiveAndEnabled;

        bool value2 = botList[0].GetComponent < LilBotNamespace.MovementControls>();

        Debug.Log(value);
        Debug.Log(value2);
        */
    }

// Update is called once per frame
void Update () {
	
	}

public void addBot(GameObject bot)
    {
        botList.Add(bot);
        int index = botList.Count - 1;
        nameDict.Add(bot.name, index);
    }

public void startMoveTo(GameObject bot, Vector3 target)
    {
        LilBotNamespace.BotQueue queue = bot.GetComponent<LilBotNamespace.BotQueue>();

        GameObject goTo = new GameObject("target");
        goTo.transform.position = target;
        goTo.AddComponent<BoxCollider>();

        queue.tr = goTo.transform;
        queue.enabled = true;
    }

public void testCall()
    {

        foreach (GameObject bot in botList)
        {
            bot.GetComponent<HeadTracking>().target = GameObject.Find("Ball").transform;
            if (bot.name != "Bob")
            {
                bot.GetComponent<FaceControls>().mouth = FaceControls.MouthState.frowning;
            }
        }

    }
}
