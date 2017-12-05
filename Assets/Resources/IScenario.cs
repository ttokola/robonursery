using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScenario
{
    void EnableScenario(bool enabled);
    void ResetScenario();
    string[] GetRequirements();

}

