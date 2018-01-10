using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentProto : AgentParser {
    //Write your override functions here.
    //With them you can automate building you robot. See below for an examples

    override public Component InitializationSettings(Component component)
    {
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

        
        //Connect to parent
        if (component.PartName.Contains("Axle") || component.PartName.Contains("Head") || component.PartName == "Neck")
        {
            component.ConnectJointTo = Component.ConnectJointTo_.ConnectToParent;

        }
        //Connect to grandParent
        if (component.PartName.Contains("Arm") || component.PartName == "RWheel" || component.PartName == "LWheel")
        {
            component.ConnectJointTo = Component.ConnectJointTo_.ConnectToGrandparent;
        }

        //hingeJoints
        if (component.PartName.Contains("Axle") || component.PartName == "LWheel" || component.PartName == "RWheel")
        {
            component.JointType = Component.JointType_.HingeJoints;
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
            //This locks the robot in to a standing position. Remove these and the robot behaves more naturally 
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