/*
    Configure loading behaviour near a loading dock
*/

using UnityEngine;
using System.Collections;

public class DockLoader : MonoBehaviour {
 
    private int trigCount = 0;
    
	private LilBotNamespace.Battery bat;
    
	void OnTriggerStay (Collider other)
	{
		bat = other.transform.root.gameObject.GetComponent<LilBotNamespace.Battery> ();
        bat.Charge(10);
	}
    
    void OnTriggerEnter (Collider other)
	{
		trigCount++;
	}
    
    void OnTriggerExit (Collider other)
	{
		trigCount--;
	}
    
    public bool IsFree ()
    {
        return (trigCount == 0) ? true : false;
    }
}