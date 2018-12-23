using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorScript
{
    //C:\>"C:\Program Files\Unity\Editor\Unity.exe" -batchmode  -projectPath "C:Repos\ML_RollerBall" -executeMethod EditorScript.PerformBuild -quit
    static void PerformBuild()
    {
        string[] scenes = { "Assets/Scenes/NewScene.unity" };
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            locationPathName = "C:\\Repos\\ml-agents\\RollBall\\RollBall.exe",
            scenes = new string[] { "Assets/Scenes/NewScene.unity" },
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        Debug.Log("Start build");
        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if(buildReport.summary.totalErrors > 0)
        {
            Debug.Log("End build with errors");
            EditorApplication.Exit(1);
        }
        Debug.Log("End build");
        Debug.Log("Output path:");
        Debug.Log(buildReport.summary.outputPath);
        EditorApplication.Exit(0);
    }
}
