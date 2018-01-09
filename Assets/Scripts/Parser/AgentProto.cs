using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentProto : AgentParser {

    override public Component InitializationSettings(Component component)
    {

        //Here we set the default values for all the components variables.
        ////////
        ///If you want to set default values to certain variables based on their name make if statements here which sets the value.
        ///This can be a handly way to speed up the process of deploying new robots. GameObjects name can be obtained from PartName.Contains(string name) see below for example.
        ///////See below for examples 
        //Movable
        if (component.PartName == "LWheel" || component.PartName.Contains("Arm") || component.PartName == "RWheel" || component.PartName == "Head")
        {
            component.Movable = true;

        }

        //MeshColliders 
        if (component.PartName.Contains("Wheel") && component.Movable == true)
        {
            component.Collider = Component.Collider_.MeshCollider;

        }

        //BoxColliders
        if (component.PartName.Contains("Arm") || component.PartName.Contains("Neck") || component.PartName.Contains("Head"))
        {
            component.Collider = Component.Collider_.BoxCollider;
        }

        //AddTorqueMotor
        if (component.PartName.Contains("Wheel"))
        {
            component.Motor = Component.Type_.AddTorque;

        }
        //Connect to parent
        if (component.PartName.Contains("Axle") || component.PartName.Contains("Head") || component.PartName == "Neck")
        {
            component.ConnectJointTo = Component.Link_.ConnectToParent;

        }
        //Connect to grandParent
        if (component.PartName.Contains("Arm") || component.PartName == "RWheel" || component.PartName == "LWheel")
        {
            component.ConnectJointTo = Component.Link_.ConnectToGrandparent;
        }

        //hingeJoints
        if (component.PartName.Contains("Axle") || component.PartName == "LWheel" || component.PartName == "RWheel")
        {
            component.JointType = Component.Connection_.HingeJoints;
        }



        return component;
    }

    override public void ConfigurableJointSettings(Component component, ConfigurableJoint joint)
    {
        joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        if (component.PartName == "Neck")
        {
            joint.angularZMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
        }

        if (component.PartName == "Head")
        {
            joint.angularZMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = ConfigurableJointMotion.Limited;
        }
        joint.anchor = new Vector3(0f, 0f, 0f);

    }

    public override void RigidbodySettings(GameObject gameobject)
    {

        if (gameobject.name == "Body")
        {    
            gameobject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
        }
    }
    public override void HingeJointSettings(Component component, HingeJoint joint)
    {
        joint.anchor = new Vector3(0f, 0f, 0f);
    }

    public override void MeshColliderSettings(GameObject gameobject)
    {
        MeshCollider mesh = gameobject.GetComponent<MeshCollider>();
        mesh.convex = true;
        //Mesh colliders sometimes don't appear without this. Remove/add when needed 
        mesh.inflateMesh = true;
    }
}