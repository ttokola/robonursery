using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentProto : AgentParser {

    public override Component AutomateConfig(Component component)
    {

        //Here we set the default values for all the components variables.
        //
        //If you want to set default values to certain variables based on their name make if statements here which sets the value.
        //This can be a handly way to speed up the process of deploying new robots. GameObjects name can be obtained from PartName.Contains(string name) see below for example.
        //See below for examples 

        component.ConnectJointTo = Component.Link_.None;
        if (component.PartName.Contains("Body"))
        {
            component.gameObject.AddComponent<Rigidbody>();
            component.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        }

        if (component.PartName.Contains("_movable"))
        {
            component.Movable = true;
            component.Collider = Component.Collider_.BoxCollider;

        }

        //set default values to 0
        component.DimensionMultipliers = new Vector3(0, 0, 0);

        //sets components with name containing wheel and movable variable true to have mesh_colliders value true. 
        if (component.PartName.Contains("Wheel_movable") && component.Movable == true)
        {
            component.ConnectJointTo = Component.Link_.ConnectToGrandparent;
            component.DimensionMultipliers = new Vector3(1, 0, 0) * AddTorqueMultiplier;
            component.Collider = Component.Collider_.MeshCollider;
            component.Motor = Component.Type_.AddTorque;
        }
        //set joint type for wheel and axes
        if (component.PartName.Contains("Wheel_movable") || component.PartName.Contains("Axle"))
        {
            component.JointType = Component.Connection_.HingeJoints;
        }

        //Set joint links 
        if (component.PartName.Contains("Axle") || component.PartName.Contains("Neck") || component.PartName.Contains("Head") || component.PartName.Contains("Tooth") || component.PartName.Contains("Eye"))
        {
            //Ruttunen's structure requires that these be linked to parent object
            component.ConnectJointTo = Component.Link_.ConnectToParent;
        }
        if (component.PartName.Contains("Arm"))
        {
            //Ruttunen's structure requires that these be linked to grandparent object
            component.DimensionMultipliers = new Vector3(0, 1, 1) * AddRelativeTorqueMultiplier;
            component.ConnectJointTo = Component.Link_.ConnectToGrandparent;
            component.Motor = Component.Type_.AddRelativeTorque;
        }
        //Set default axes
        if (component.PartName.Contains("Neck") || component.PartName.Contains("Head"))
        {
            //Free 3D rotation for neck and head
            component.DimensionMultipliers = new Vector3(1, 1, 1) * AddTorqueMultiplier;
        }
        if (component.PartName.Contains("Eye"))
        {
            //X and Y axes for eyes
            component.DimensionMultipliers = new Vector3(1,1,0)*AddRelativeTorqueMultiplier;
        }

        

        //Set default values to components variables


        return component;
    }



}