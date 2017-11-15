using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need to find other way to get the movementcontrols to this script
using LilBotNamespace;

#if UNITY_EDITOR
using UnityEditor;
#endif




public abstract class AgentParser : MonoBehaviour {

    

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
        public float[] DimensionMultipliers;
        public int[] ActionIndeces;
    }

    [System.Serializable]
    public struct AgentParameters
    {
        //public GameObject[] InUse;

    }

    // Use this for initialization
    private MovementControls motor;
    
   

    public AgentParameters agentParameters;

    public Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
    /**< \brief Keeps track of the agents which subscribe to this proto*/

    [SerializeField]
    public List<Component> components = new List<Component>();


    void Start()
    {
        Debug.Log("Start run");
        foreach (Component component in components)
        {
            if (component.gameObject.GetComponent<Rigidbody>() == null && component.Movable)
            {
                component.gameObject.AddComponent<Rigidbody>();
                component.gameObject.AddComponent<BoxCollider>();
                if (component.Type == Component.Type_.Joint)
                {
                    component.gameObject.AddComponent<ConfigurableJoint>();
                }
            }
        }
    }

    public List<Component> GetComponents()
    {
        return components;
    }
    
    public Component GetComponentByName(string name)
    {
        Component result = new Component();
        foreach(Component component in components)
        {
            if (component.PartName == name)
            {
                return component;
            }
        }
        Debug.LogError("No component exists by given name");
        return result;
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
                component.Movable = false;
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
        //
        

        components.Clear();
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != allChildren[0])
            {
                var component = new Component();
                component.PartName = child.name;
                component.gameObject = child.gameObject;
                component.Movable = false;

                component.ActionIndeces = new int[3];
                component.DimensionMultipliers = new float[3];
                components.Add(component);
            }
        }
    }

    public void MoveMovableParts(float[] act)
    {

        motor = this.gameObject.GetComponent<MovementControls>();
        foreach (Component component in GetComponents())
        {
            if(component.Movable == true)
            {
                switch (component.Type)
                {

                    case Component.Type_.Wheel: {
                            motor.Wheel(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[component.ActionIndeces[0]]);
                            break;
                        }
                    
                    case Component.Type_.Joint:
                        {
                            for(int i=0; i<component.DimensionMultipliers.Length; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0)
                                {
                                    motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[component.ActionIndeces[i]]*component.DimensionMultipliers[i],i);

                                }
                            }
                            
                         //   motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(),);
                            break;
                        }

                }


            }
        }
    }



	
}
