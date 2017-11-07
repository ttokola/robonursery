using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public struct Component
{
    public string PartName;
    public GameObject gameObject;
    public bool Movable;
    public enum Type_
    {
        Joint,
        Wheel
    }
    public Type_ Type;
    public Vector3 DimensionMultipliers;
    public int[] ActionIndeces;
}

[System.Serializable]
public struct AgentParameters
{
    public GameObject[] InUse;

}

public abstract class AgentParser : MonoBehaviour {

    // Use this for initialization
    public AgentParameters agentParameters;

    public Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
    /**< \brief Keeps track of the agents which subscribe to this proto*/

    [SerializeField]
    public List<Component> components = new List<Component>();

    public List<Component> GetComponents()
    {
        return components;
    }
    

    public Dictionary<int, List<Component>> CollectComponents()
    {
        Dictionary<int, List<Component>> result = new Dictionary<int, List<Component>>();
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != allChildren[0])
            {
                var component = new Component();
                component.PartName = child.name;
                component.gameObject = child.gameObject;
                component.Movable = true;
                components.Add(component);
            }
        }
        return result;
    }
	void OnEnable()
    {
        //id = gameObject.GetInstanceID();

    }
    //This is called by the Academy at the start of the environment
    [ContextMenu("Initialize AgentProto")]
    void InitializeParser()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != allChildren[0])
            {
                var component = new Component();
                component.gameObject = child.gameObject;
                component.Movable = true;
                components.Add(component);
            }
        }
    }
	
}
