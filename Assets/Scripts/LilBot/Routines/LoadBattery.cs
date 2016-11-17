// Autonomous actions which can possibly override anything else, like manual control or queue executing

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class LoadBattery : MonoBehaviour 
{
    public Battery battery;
    public DockLoader[] loadingAreas;
    public MovementControls movementControls;
    
    private Vector3 destination;
	
	int Execute ()
    // Attempt to load at a loading station
    // Return codes:
    // 0: Successfully loaded or already full battery
    // 1: moving towards loading station
    // 2: Cannot find a free loading station
	{
        if (battery.normLevel >= 1.0f)
        {
			return 0;
		}
        var noFree = true;
        foreach (var area in loadingAreas)
        {
            if (area.IsFree())
            {
                destination = area.transform.position;
                noFree = false;
            }
        }
        if (noFree)
        {
            return 2;
        }
        movementControls.DriveTo(destination);
		return 1;
	}
}

} // End namespace