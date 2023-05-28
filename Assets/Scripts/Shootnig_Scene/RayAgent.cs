using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RayAgent : Agent
{
    public Transform Target;

    public float moveSpeed = 1.5f;
    public float turnSpeed = 200.0f;

    public override void Initialize()
    {
        MaxStep = 5000;
        Target = GameObject.Find("Target").GetComponent<Transform>();
    }

    public override void OnEpisodeBegin()
    {
        this.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-4, 4), 1.0f, Random.Range(-4, 4));
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        Target.transform.localPosition = new Vector3(Random.Range(-4, 4), 1.0f, Random.Range(-4, 4));
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(transform.localRotation.y);
        
        sensor.AddObservation(Target.localRotation.x);
        sensor.AddObservation(Target.localPosition.z);
        sensor.AddObservation(Target.localRotation.y);

    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        Vector3 dir = Vector3.zero;
        Vector3 rot =  Vector3.zero;

        switch (action[0])
        {
            case 1: dir = transform.forward; break;
            case 2: dir = -transform.forward; break;
        }

        switch(action[1])
        {
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }

        transform.Rotate(rot, Time.deltaTime * turnSpeed);
        this.GetComponent<Rigidbody>().AddForce(dir * moveSpeed, ForceMode.VelocityChange);

        AddReward(-1 / (float)MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;
        actionsOut.Clear();

        if(Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
        }
        if(Input.GetKey(KeyCode.S))
        {
            action[0] = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            AddReward(-1.0f / MaxStep); // 수정
            EndEpisode();
        }
        if (collision.collider.CompareTag("Target"))
        {
            //rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
            //Destroy(collision.gameObject);
            AddReward(1.0f / MaxStep); // 수정
            EndEpisode();
            //Instantiate(Target, new Vector3(Random.Range(-4, 4), 0.5f, Random.Range(-4, 4)), Quaternion.identity);
        }
    }
}
