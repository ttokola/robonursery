using System;
using System.IO;
using UnityEngine;
using System.Reflection;


public class AgentGenerator : MonoBehaviour {
    
    

    [ContextMenu("Generate MLA agent file")]
    void DoSomething()
    {
        //set path to MLAagent.cs
        string filename = Environment.CurrentDirectory + @"\Assets\Scripts\MLA\MLAagent.cs";


        agentParser agentParser = this.gameObject.GetComponent<agentParser>();
       // Component[] agentComponents2 = agentParser.GetComponents(typeof(agentParser.Component));

//        Debug.Log("############"+agentComponents2);
        //read current file
        string text = File.ReadAllText(filename);
        //declare variables
        Component[] component_list;
        //component_list = agentParser.get();
        string variable;
        string declaration;
        string declaration_start_string = "public class MLAagent : Agent {";
        int declaration_start=text.IndexOf(declaration_start_string)+declaration_start_string.Length;

        string collect_state_start_string = "List<float> state = new List<float>();";
        int collect_state_start = text.IndexOf(collect_state_start_string) + collect_state_start_string.Length;
        string collect_state_insertion = "\n//Insert collect state variables here";

        string agent_step_start_string = "public override void AgentStep(float[] act)\n    {";
        int agent_step_start = text.IndexOf(agent_step_start_string) + agent_step_start_string.Length;
        //this is a place holder until aku is done
        string motor;
        //remove
        variable = "TEST_VARIABLE";
        
        foreach (Component comp in component_list);
        {
            //write declaration of the variable.
            variable = comp;
            declaration = " \npublic float " + variable + ";";
            text = text.Insert(declaration_start, declaration);

            //insert collect state stuff here if needed 
            text = text.Insert(collect_state_start, collect_state_insertion);
            
            //agent step.
            



        }
        


        





        Debug.Log(text);
        /*/REMOVE LATER
        float a=agentParser.LUpperArm.Axes;

        FieldInfo[] agentComponents = agentParser.GetType().GetFields();

        // Component c = agentParser.GetComponent(agentComponents[5]);

        foreach (System.Reflection.MemberInfo m in typeof(agentParser.Component).GetMembers())
            Debug.Log(m.Name);

        Debug.Log("TESTING " );
        foreach (FieldInfo component in agentComponents)
        {
            
         Debug.Log("Name: "+component.Name +"\nValue: "+component.GetValue(agentParser));
        

        }
        ///^^ REMOVE LATER*/
        

    }
}
