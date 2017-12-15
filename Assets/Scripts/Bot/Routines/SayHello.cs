/*
    Turn to object, say hello to it and wave your hand, and possibly send request to wave back if opponent is actor bot
    
    Use setTask method to give task to actor bot
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class SayHello : MonoBehaviour {

    [Tooltip("Drag the ArmControls script attached to the robot here")]
    public ArmControls armControls;
    [Tooltip("Drag the MovementControls script attached to the robot here")]
    public MovementControls movementControls;

    private float movementRange;
    private GameObject helloTarget;

    public int state;
    private int waves;
    private int wavecounter;

    private bool askResponse;

    private AudioSource helloVoiceSource;
    public AudioClip helloSound;

	void Start ()
    {
        helloVoiceSource = GetComponentInChildren<AudioSource>();
	}
	
	void Update ()
        /* Task execution working as state machine
         * If state is 0 then execution does not happen
         */
    {
        switch (state)
        {
            case 0:
                break;
            case 1:
                armControls.SetStaticPosition("idle");
                if (movementControls.TurnTo(helloTarget.transform) == 0)
                {
                    state = 2;
                    helloVoiceSource.PlayOneShot(helloSound);
                }
                break;

            case 2:
                if (armControls.SetStaticPosition("wavePosition1") == 0){                
                    state = 3;
                }
                movementControls.TurnTo(helloTarget.transform);
                break;
            case 3:
                if (armControls.SetStaticPosition("wavePosition2") == 0)
                {
                    wavecounter++;
                    if (wavecounter >= waves)
                    {
                        state = 4;
                    }
                    else
                    {
                        state = 2;
                    }
                    
                }
                movementControls.TurnTo(helloTarget.transform);
                break;
            case 4:
                state = 0;
                wavecounter = 0;

                if (askResponse)
                {
                    //Sending task to response to target
                    SayHello t = helloTarget.GetComponent<SayHello>();
                    if (t != null)
                    {
                        t.setTask(gameObject, 3, false);
                    }
                }
                break;
        }
	}

    public int setTask(GameObject ht, int w, bool ar)
        /*
         * Set greeting task for robot 
         * 
         * Set target, number of waves, and triggering target robots sayHello script
         */
    {
        helloTarget = ht;
        waves = w;
        wavecounter = 0;
        askResponse = ar;
        state = 1;
        return 0;
    }


    public int getState()
    {
        return state;
    }

    public void cancelTask()
    // Stop executing task
    {
        state = 0;
    }
}
