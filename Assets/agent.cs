using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents; 
using Unity.MLAgents.Sensors; 


public class agent : Agent 
{
    private Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;

    //Step 1
    public override void OnEpisodeBegin() //agent's task
    {
        if (this.transform.localPosition.y < 0) //when it falls
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero; //stop rolling
            this.rBody.velocity = Vector3.zero; //stop rolling
            this.transform.localPosition = new Vector3(0, 0.5f, 0); //reset the y position
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
    }

    //Step 2
    public override void CollectObservations(VectorSensor sensor) 
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition); //target location
        sensor.AddObservation(this.transform.localPosition); //agent's location

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    //Step 3
    public float speed = 10;
    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        // Rewards 
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Case 1. Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f); //give reward
            EndEpisode(); //signal that the task is over
        }

        // Case 2. Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode(); //signal that the task is over
        }
    }
    
    //user input to controll the agent
    public override void Heuristic(float[] actionsOut) 
    {
        actionsOut[0] = Input.GetAxis("Horizontal"); //keyboard input left/right
        actionsOut[1] = Input.GetAxis("Vertical"); //keyboard input up/dowbn
    }
}