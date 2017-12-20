﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;
using System;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.IO;

public abstract class AgentParser : MonoBehaviour {
    //Set default values for the Components variables.Change these if you want the initialize parser function to give different default values
    //If you want to set certain values to different components based on their name or some other factor see function AutomateConfig() at the bottom 
    private bool DefaultMovable=false;
    private Component.Type_ DefaultMotor = Component.Type_.AddRelativeTorque;
    private Component.Link_ DefaultConnectJointTo= Component.Link_.ConnectToGrandparent;
    private Component.Connection_ DefaultJointType= Component.Connection_.ConfigurableJoints;
    private Component.Collider_ DefaultCollider= Component.Collider_.BoxCollider;
    private Vector3 DefaultDimensionMultiplers = new Vector3(0, 0, 0);
    //Note that if you set this vectors default values to be >=0 it might cause unintended interactions with the agent.
    private Vector3 DefaultActionIndicies = new Vector3(-1, -1, -1);

        



    [System.Serializable]
    public class Component
    {
        //variables used by ResetAgentPose()
        [HideInInspector]
        public Vector3 transformsPosition;
        [HideInInspector]
        public Quaternion transformsRotation;
        //All enum type variable are declared here
        public enum Type_
        {
            AddRelativeTorque,
            AddTorque
        }
        public enum Connection_
        {

            ConfigurableJoints,
            HingeJoints
        }
        public enum Link_
        {

            None,
            ConnectToParent,
            ConnectToGrandparent

        }
        public enum Collider_
        {

            None,
            BoxCollider,
            MeshCollider,
            SphereCollider,
            CapsuleCollider

        }
        //Variables visible at the inspector UI
        [Tooltip("Name of the gameobject")]
        public string PartName;
        [Tooltip("Link to the gameobject")]
        public GameObject gameObject;
        [Tooltip("Sets object to movable.")]
        public bool Movable;
        [Tooltip("If the part is movable it uses this type of motor to move")]
        public Type_ Motor;
        [Tooltip("Set where you want to connect this object. This only matters if you run the script with Link flag")]
        public Link_ ConnectJointTo;
        [Tooltip("If you connect this object to something it is done with this type of joint")]
        public Connection_ JointType;
        [Tooltip("Set what type of collider you want to add to the object. Only matter when you run the script with AddColliders flag")]
        public Collider_ Collider;
        [Tooltip("Add multiplier to X,Y,Z movement.")]
        public Vector3 DimensionMultipliers;
        [Tooltip("Set what action from the AI corresponds to which above movements")]
        public Vector3 ActionIndeces;
        
        
        
    }

    

    // Use this for initialization
    private MovementControls motor;
    private string motorname1 = "Wheel";
    [Tooltip("When script export config is run from context menu it creates a text file with this name")]
    public string ConfigName = "Config";
    [Tooltip("When dimension multiplier flag active, add this value to object with AddTorque motor ")]
    public int AddTorqueMultiplier = 1;
    [Tooltip("When dimension multiplier flag active, add this value to object with AddRelativeTorque motor ")]
    public int AddRelativeTorqueMultiplier = 1;

    //Parser flags
    [Tooltip("Check to run checked attributes on start. With this you dont have to run the script from context menu. All the joint, colliders etc also disappear when session is stopped")]
    public Boolean Run;
    [Tooltip("Adds colliders to checked objects")]
    public Boolean AddColliders;
    [Tooltip("Links compponents which link attribute is true to their parent")]
    public Boolean Link;
    [Tooltip("Automatically assigns autoindicies to movable objects")]
    public Boolean AutoSetActionIndicies;
    [Tooltip("Automatically assign dimension multiplier to movable objects")]
    public Boolean AutoSetDimensionMultipliers;
    public int actsize;
    //[Tooltip("Instead of linking the components with link attribute true to their parents, instead link them to their grand parent")]
    // public Boolean Link_grandparent;



    public Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
    /**< \brief Keeps track of the agents which subscribe to this proto. NOT IN USE*/

    [SerializeField]
    public List<Component> components = new List<Component>();

    
    [HideInInspector]
    protected Vector3 defaultJointAxes = new Vector3(0, 1, 1);
    

    [HideInInspector]
    protected Vector3 defaultWheelAxes = new Vector3(0, 1, 1);

    void Start()
    {

        if (Run == true)
        {
            foreach(Component component in components){
                component.transformsPosition = component.gameObject.transform.position;
                component.transformsRotation = component.gameObject.transform.rotation;
            }
            AddIndeces();
        }

    }

    public List<Component> GetComponents()
    {
        return components;
    }

    public Component GetComponentByName(string name)
    {
        Component result = new Component();
        foreach (Component component in components)
        {
            if (component.PartName == name)
            {
                return component;
            }
        }
        Debug.LogError("No component exists by given name");
        return result;
    }

    private Dictionary<int, List<Component>> CollectComponents()
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
   
    [ContextMenu("Initialize AgentProto")]
    void InitializeParser()
    {
        //
        if (this.gameObject.GetComponent<MovementControls>() == null)
        {
            this.gameObject.AddComponent<MovementControls>();
        }

        components.Clear();



        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != allChildren[0])
            {
                
                var component = new Component();
                
                component.PartName = child.name;
                component.gameObject = child.gameObject;
                component.transformsPosition = child.position;
                component.transformsRotation = child.rotation;
                //set default values set them at the beginning   
                component.Movable= DefaultMovable;
                component.Motor = DefaultMotor;
                component.ConnectJointTo = DefaultConnectJointTo;
                component.JointType = DefaultJointType;
                component.Collider = DefaultCollider;
                component.DimensionMultipliers = DefaultDimensionMultiplers;
                component.ActionIndeces = DefaultActionIndicies;


                component =AutomateConfig(component);


                components.Add(component);
            }
        }
    }

    [ContextMenu("Run the flagged operations")]
    void AddIndeces()
    {

        motor = this.gameObject.GetComponent<MovementControls>();
        int number = 0;
        foreach (Component component in GetComponents())
        {
            if (component.Movable == true)
            {
                
                    CheckRigidbody(component.gameObject, component.Collider);

                switch (component.Motor)
                {

                    case Component.Type_.AddTorque:
                        {
                            if (AutoSetDimensionMultipliers == true)
                            {
                                WheelMultipliers(component);
                                
                            }
                            if (AutoSetActionIndicies == true)
                            {
                                number = WheelAction_Indicies(component,number);
                                
                            }
                            break;
                        }

                    case Component.Type_.AddRelativeTorque:
                        {
                            if (AutoSetDimensionMultipliers == true)
                            {
                                JointMultipliers(component);
                                
                               
                            }
                            if (AutoSetActionIndicies == true)
                            {
                               number = JointAction_Indicies(component, number);

                            }
                            break;
                        }

                }

            }
            if ((component.ConnectJointTo != Component.Link_.None) && Link == true)
            {
                if (component.JointType == Component.Connection_.ConfigurableJoints) {
                    LinkWithBallJoints(component);
                }
                else if (component.JointType == Component.Connection_.HingeJoints)
                {
                    LinkWithHingeJoints(component);
                }
            }
        }
        actsize = number;
    }
    [ContextMenu("Export config file")]
    public void ExportConfig()
    {
        components = GetComponents();
        string data;
        string path = Environment.CurrentDirectory + @"\Assets\Scripts\Parser\" + ConfigName + ".txt";
        Debug.Log("Config saved to " + path);
        //clear file
        File.WriteAllText(path, String.Empty);

        using (StreamWriter file = new System.IO.StreamWriter(path, true)) {
            file.WriteLine(AddTorqueMultiplier);
            file.WriteLine(AddRelativeTorqueMultiplier);

            foreach (Component component in components) {
                data = component.PartName + ";" + component.Movable + ";" + component.Motor + ";" + component.DimensionMultipliers[0] + ";" + component.DimensionMultipliers[1] + ";" + component.DimensionMultipliers[2] + ";" + component.ActionIndeces[0] + ";" + component.ActionIndeces[1] + ";" + component.ActionIndeces[2] + ";";
                file.WriteLine(data);
            }
            file.Flush();
        }



    }


    //Moves all movable parts
    public void MoveMovableParts(float[] act)
    {
        if (this.gameObject.GetComponent<MovementControls>() == null)
        {
            this.gameObject.AddComponent<MovementControls>();
        }
        motor = this.gameObject.GetComponent<MovementControls>();
        foreach (Component component in GetComponents())
        {
            if (component.Movable == true)
            {

                switch (component.Motor)
                {

                    case Component.Type_.AddTorque: {
                            for (int i = 0; i < 3; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0 && component.ActionIndeces[i] >= 0)
                                {
                                    motor.Wheel(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[(int)component.ActionIndeces[i]] * component.DimensionMultipliers[i], i);

                                }
                            }
                            break;

                        }

                    case Component.Type_.AddRelativeTorque:
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0 && component.ActionIndeces[i] >= 0)
                                {
                                    motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[(int)component.ActionIndeces[i]] * component.DimensionMultipliers[i], i);

                                }
                            }

                            break;
                        }

                }

            }
        }
    }

    private void LinkWithBallJoints(Component component)
    {
        
        if (component.gameObject.GetComponent<ConfigurableJoint>()== null)
        {
            component.gameObject.AddComponent<ConfigurableJoint>();
        }
        ConfigurableJoint joint = component.gameObject.GetComponent<ConfigurableJoint>();
        joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        

        
        //Add rigidbody and colliders + connects the object to its parent/grandparent

        if ((component.ConnectJointTo == Component.Link_.ConnectToGrandparent) && component.gameObject.transform.parent.parent.gameObject != this.gameObject)
        {
            CheckRigidbody(component.gameObject.transform.parent.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
           // joint.connected = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
        }
        else if ((component.ConnectJointTo == Component.Link_.ConnectToParent) && component.gameObject.transform.parent.gameObject != this.gameObject)
        {
            CheckRigidbody(component.gameObject.transform.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.GetComponent<Rigidbody>();
            //joint.connected = component.gameObject.transform.parent.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log("There seems to be a issue with linking in object " + component.PartName);
           
        }
        joint.anchor = new Vector3(0f, 0f, 0f);
    }

    private void LinkWithHingeJoints(Component component)
    {
        CheckRigidbody(component.gameObject, component.Collider);
        
        if (component.gameObject.GetComponent<HingeJoint>() == null)
        {
            component.gameObject.AddComponent<HingeJoint>();    
        }
        HingeJoint joint = component.gameObject.GetComponent<HingeJoint>();
        
        //Add rigidbody to parent if it doesn't have it

        if ((component.ConnectJointTo == Component.Link_.ConnectToGrandparent) && component.gameObject.transform.parent.parent.gameObject != this.gameObject)
        {
                
            CheckRigidbody(component.gameObject.transform.parent.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
            joint.connectedAnchor = component.gameObject.transform.position;
        }
        else if ((component.ConnectJointTo == Component.Link_.ConnectToParent) && component.gameObject.transform.parent.gameObject != this.gameObject)
        {
            CheckRigidbody(component.gameObject.transform.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.GetComponent<Rigidbody>();
            joint.connectedAnchor = component.gameObject.transform.position;

        }
        else
        {
            Debug.Log("There seems to be a issue with linking in object " + component.PartName + " Check if you have both link_parent and link_to_grandparent active");
            
        }
        //force anchor to be 0 0 0. This makes the wheels connect properly
        joint.anchor = new Vector3(0f,0f,0f);
        }

    private void CheckRigidbody(GameObject gameobject, Component.Collider_ collider)
    {
        if (gameobject.GetComponent<Rigidbody>() == null)
        {
            gameobject.AddComponent<Rigidbody>();
        }
        
       
        if (AddColliders == true && gameobject.GetComponent<BoxCollider>() == null && gameobject.GetComponent<SphereCollider>() == null && gameobject.GetComponent<MeshCollider>() == null)
        {
            Debug.Log("AddingCollider to" + gameobject.name);

            switch (collider)
            {
                case Component.Collider_.BoxCollider:
                    {
                        gameobject.AddComponent<BoxCollider>();
                        //get colliders size from Renderer
                        if (gameobject.GetComponent<Renderer>() != null)
                        {
                            //component.GetComponent<BoxCollider>().size = component.GetComponent<Renderer>().bounds.size;

                            Debug.Log(gameobject.name + gameobject.GetComponent<Renderer>().bounds.size);
                        }
                        //set default small values if renderer does not exist. For example in case of bones
                        else
                        {
                            Vector3 vector = new Vector3(0.5f, 0.5f, 0.5f);
                            gameobject.GetComponent<BoxCollider>().size = vector;
                        }
                        break;
                    }
                case Component.Collider_.MeshCollider:
                    {
                        MeshCollider mesh = gameobject.AddComponent<MeshCollider>();
                        mesh.convex = true;
                        //Mesh colliders sometimes don't appear without this. Remove/add when needed 
                        mesh.inflateMesh = true;
                        break;
                    }
                case Component.Collider_.CapsuleCollider:
                    {
                         gameobject.AddComponent<CapsuleCollider>();
                        break;
                    }
                case Component.Collider_.SphereCollider:
                    {
                        gameobject.AddComponent<SphereCollider>();
                        break;
                    }


            }

           
        }
    } 

    public void ResetAgentPose(Vector3 position)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Component component in components)
        {
            component.gameObject.transform.position = component.transformsPosition+position;
            component.gameObject.transform.rotation = component.transformsRotation;
            if (component.Movable)
            {
                component.gameObject.GetComponent<Rigidbody>().velocity = default(Vector3);
                component.gameObject.GetComponent<Rigidbody>().angularVelocity = default(Vector3);
            }

        }
    }

    public void ResetAgentPose()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Component component in components)
        {
            component.gameObject.transform.position = component.transformsPosition;
            component.gameObject.transform.rotation = component.transformsRotation;
            if (component.Movable)
            {
                component.gameObject.GetComponent<Rigidbody>().velocity = default(Vector3);
                component.gameObject.GetComponent<Rigidbody>().angularVelocity = default(Vector3);
            }

        }
    }
    public virtual void WheelMultipliers(Component component)
    {
        if ((AutoSetActionIndicies && AutoSetDimensionMultipliers) == true) {
            component.DimensionMultipliers.x = AddTorqueMultiplier;
            }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if(component.ActionIndeces[i] >= 0){
                    component.DimensionMultipliers[i] = AddTorqueMultiplier; 
            }
            }
        }
    }
    public virtual int WheelAction_Indicies(Component component, int number) {
        if ((AutoSetActionIndicies && AutoSetDimensionMultipliers) == true) {

            component.ActionIndeces.Set(number, -1, -1);

            number = number + 1;
        }
        else {
            for (int i = 0; i < 3; i++)
            {
                if (component.DimensionMultipliers[i] > 0)
                {
                    component.ActionIndeces[i] = number;
                    number = number + 1;
                }

            }
            }
        return number;
    }

     public virtual void JointMultipliers(Component component)
    {
        if ((AutoSetActionIndicies && AutoSetDimensionMultipliers) == true)
        {
            component.DimensionMultipliers.y = AddRelativeTorqueMultiplier;
            component.DimensionMultipliers.z = AddRelativeTorqueMultiplier;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.ActionIndeces[i] >= 0)
                {
                    component.DimensionMultipliers[i] = AddRelativeTorqueMultiplier;
                }
            }
        }

    }

    public virtual int JointAction_Indicies(Component component, int number)
    {
        if ((AutoSetActionIndicies && AutoSetDimensionMultipliers) == true)
        {
            component.ActionIndeces.Set(-1, number, number+1);

            number = number + 2;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.DimensionMultipliers[i] > 0)
                {
                    component.ActionIndeces[i] = number;
                    number = number + 1;
                }

            }
        }
        return number;

    }

    public virtual Component AutomateConfig(Component component)
    {
        /*
        //Here we set the default values for all the components variables.
                ////////
                ///If you want to set default values to certain variables based on their name make if statements here which sets the value.
                ///This can be a handly way to speed up the process of deploying new robots. GameObjects name can be obtained from PartName.Contains(string name) see below for example.
                ///////See below for examples 
        if (component.PartName.Contains("_movable"))
        {
            component.Movable = true;
            component.Collider = Component.Collider_.BoxCollider;

        }

        //sets components with name containing wheel and movable variable true to have mesh_colliders value true. 
        if (component.PartName.Contains("Wheel") && component.Movable == true)
        {
            component.Collider = Component.Collider_.MeshCollider;
        }


        //set default values to 0
        component.DimensionMultipliers = new Vector3(0, 0, 0);


        //Sets variables to certain values depending on their name
        if (component.PartName.Contains(motorname1) || component.PartName.Contains("Axle"))
        {
            component.Motor = Component.Type_.AddTorque;
        }

        if (component.PartName.Contains("Axle") || component.PartName.Contains("Neck") || component.PartName.Contains("Head") || component.PartName.Contains("Tooth") || component.PartName.Contains("Eye"))
        {
            component.ConnectJointTo = Component.Link_.ConnectToParent;
        }

        if (component.PartName.Contains("Arm") || component.PartName.Contains("Wheel_movable"))
        {
            component.ConnectJointTo = Component.Link_.ConnectToGrandparent;
        }*/
         
    //Set default values to components variables
  

        return component;
    }


    
    /*Add default axis multipliers based on the axes that have action_indeces set*/
    /*
    public virtual int AddMultipliers(Component component)
    {
        if ((Action_Indicies && Multipliers) == true)
        {
            if (component.Motor == Component.Type_.AddRelativeTorque)
            {
                //component.DimensionMultipliers.y = JointMultiplier;
                //component.DimensionMultipliers.z = JointMultiplier;
                component.DimensionMultipliers = WheelMultiplier * defaultJointAxes;
            }
            if (component.Motor == Component.Type_.AddTorque)
            {
                //component.DimensionMultipliers.x = WheelMultiplier;
                component.DimensionMultipliers = WheelMultiplier * defaultWheelAxes;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.ActionIndeces[i] >= 0)
                {
                    if (component.Motor == Component.Type_.AddRelativeTorque)
                    {
                        component.DimensionMultipliers[i] = JointMultiplier;
                    }
                    if (component.Motor == Component.Type_.AddTorque)
                    {
                        component.DimensionMultipliers[i] = WheelMultiplier;
                    }
                    
                }
            }
        }

        return -1;
    }

    public virtual int AddActionIndeces(Component component)
    {
        if (component.Motor == Component.Type_.AddRelativeTorque)
        {
            if ((Action_Indicies && Multipliers) == true)
            {
                
                Vector3 a_i = Vector3.Scale(defaultJointAxes, new Vector3(1f, 1f, 1f));
                component.ActionIndeces.Set(actsize, -1, -1);

                actsize += 1;
            }
        }
        for (int i=0; 0<3; i++)
        {
            if (component.DimensionMultipliers[i] >= 0)
            {
                component.ActionIndeces[i] = actsize;
                actsize += 1;
            }
        }
        return -1;
    }
    */
}
