using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class SayHello : MonoBehaviour {

    public ArmControls armControls;
    public MovementControls movementControls;

    public float movementRange;
    public GameObject helloTarget;

    public int state;
    public int waves;
    private int wavecounter;

    private bool askResponse;

    private AudioSource helloVoiceSource;
    public AudioClip helloSound;

	// Use this for initialization
	void Start () {
        helloVoiceSource = GetComponentInChildren<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
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
    {
        helloTarget = ht;
        waves = w;
        wavecounter = 0;
        askResponse = ar;
        state = 1;
        return 0;
    }
}
