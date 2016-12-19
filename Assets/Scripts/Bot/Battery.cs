/*
    Internal battery object, used with bots
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{
public class Battery : MonoBehaviour
{
    [Tooltip("Maximum battery level")]
	public float max;
    [Tooltip("The material which should be recolored according to battery state")]
	public Material mat;

    private bool previouslyEmpty, previouslyFull, previouslyLow;
	[SerializeField]
	private float _level;	
    
    private AudioSource empty, lowCharge, fullCharge;
    
	public float level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = Mathf.Max(0, Mathf.Min(max, value));
		}
	}
	
	public float normLevel
	// Return battery level normalized to 0..1
	{
		get
		{
			return level/max;
		}
		set
		{
			level = max * value;
		}
	}
    
    public GameObject audioSource;
    
    void Start ()
    {
        // Can't reference directly to an audio source from another script. This
        // gets all the audio sources as an array, but the ordering has to stay
        // constant to get the right audio clip
        var soundClips = audioSource.GetComponents<AudioSource> ();
        empty = soundClips[0];
        lowCharge = soundClips[1];
        fullCharge = soundClips[2];
        
        previouslyFull = true;  // To not play full charge sound in the beginning
    }
        

	void Update ()
	{	
        // Set battery material color
		float r, g;
		if (normLevel > 0.5)
		{
			r = 1 - Mathf.InverseLerp (0.5f, 1.0f, normLevel);
			g = 1;
		}
		else
		{
			g = Mathf.InverseLerp (0.0f, 0.5f, normLevel);
			r = 1;
		}
		mat.SetColor ("_EmissionColor", new Color (r, g, 0.0f, 1.0f));
        
        // Play sound depending on charge, keep track of previous charge to avoid repeating
        
        // Full charge
        if (normLevel < 1.0f)
        {
            previouslyFull = false;
        }
        if (normLevel >= 1.0f && ! previouslyFull)
        {
            fullCharge.Play();
            previouslyFull = true;
        }
        
        // Low charge
        if (normLevel > 0.3f)
        {
            previouslyLow = false;
        }
        if (normLevel <= 0.3f && ! previouslyLow)
        {
            lowCharge.Play();
            previouslyLow = true;
        }
        
        // Empty
        if (normLevel > 0f)
        {
            previouslyEmpty = false;
        }
        if (normLevel <= 0.0f && ! previouslyEmpty)
        {
            empty.Play();
            previouslyEmpty = true;
        }       
	}
		
	public void Deplete(float amount)
	{
		level -= amount * Time.deltaTime;
	}
	
	public void Charge(float amount)
	{
		level += amount * Time.deltaTime;
	}
	

}

} // End namespace