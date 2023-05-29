using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class NoRayAgent : Agent
{
    public Transform Target;
    public Transform Obstacle;
    public float moveSpeed = 1.5f;
    public float turnSpeed = 200.0f;


    public override void OnEpisodeBegin()
    {
        this.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0.0f,1.0f,0.0f);
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        Target.transform.localPosition = new Vector3(Random.value * 14 - 7, 0.5f, Random.value * 8 - 4);
        Obstacle.transform.localPosition = new Vector3(Random.value * 14 - 7, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        sensor.AddObservation(transform.localRotation.y);
        
        sensor.AddObservation(Target.localPosition.x);
        sensor.AddObservation(Target.localPosition.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        //타겟을 찻을시 리워드점수를 주고, 에피소드를 종료시킨다.
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // 이동을 구현
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
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-1.0f); // 수정
            EndEpisode();
        }
    }
}
