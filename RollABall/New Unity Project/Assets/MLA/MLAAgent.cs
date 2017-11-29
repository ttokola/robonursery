﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLAAgent : Agent
{

    public GameObject point;
    public float speed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        state.Add(gameObject.transform.position.x);
        state.Add(gameObject.transform.position.z);
        state.Add(gameObject.transform.GetComponent<Rigidbody>().velocity.x / 5f);
        state.Add(gameObject.transform.GetComponent<Rigidbody>().velocity.z / 5f);
        state.Add(point.transform.position.x);
        state.Add(point.transform.position.z);
        return state;
    }

    public override void AgentStep(float[] act)
    {
        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            float moveHorizontal = act[0];
            float moveVertical = act[1];
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.AddForce(movement * speed);
            if (moveVertical > 5f)
            {
                reward = -0.1f;
            }
            if (moveVertical < -5f)
            {
                reward = -0.1f;
            }
            if (moveHorizontal > 5f)
            {
                reward = -0.1f;
            }
            if (moveHorizontal < -5f)
            {
                reward = -0.1f;
            }
        }
        
        if (Mathf.Abs(point.transform.position.x - gameObject.transform.position.x)<1f &&
            Mathf.Abs(point.transform.position.z - gameObject.transform.position.z)<1f)
        {
            reward = 1f;
            point.transform.position = new Vector3(Random.Range(-15f, 15f), 0.5f, Random.Range(-15f, 15f));
        }

        
        if (done == false)
        {
            if (rb.velocity.magnitude == 0f) { reward = -1f; }
        }
       
    }

    public override void AgentReset()
    {
        gameObject.transform.position = new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));

    }
    
}