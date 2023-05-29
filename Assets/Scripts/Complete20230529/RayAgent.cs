using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
//mlAgent ���� �����ؾ� ��

public class RayAgent : Agent
{
    Rigidbody rBody;
    public Transform Obstacle;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;

    public GameObject viewModel = null;

    public override void OnEpisodeBegin()
    {
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 1.0f, 0);


        //Ÿ���� ��ġ�� ���Ǽҵ� ���۽� �����ϰ� ����ȴ�.
        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 14 - 7,
                                           0.5f,
                                           Random.value * 8 - 4);
        Obstacle.localPosition = new Vector3(Random.value * 14 - 7,
                                           0.5f,
                                           Random.value * 8 - 4);
    }

    /// <summary>
    /// ��ȭ�н� ���α׷����� ���������� ����
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //Ÿ�ٰ� ������Ʈ�� �������� �����Ѵ�.
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        //���� ������Ʈ�� �̵����� �����Ѵ�.
        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    /// <summary>
    /// ��ȭ�н��� ����, ��ȭ�н��� ���� �ൿ�� �����Ǵ� ��
    /// </summary>
    public float forceMultiplier = 10;
    float m_LateralSpeed = 1.0f;
    float m_ForwardSpeed = 1.0f;

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        //Ÿ���� ������ ������������ �ְ�, ���Ǽҵ带 �����Ų��.
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        MoveAgent(actionBuffers.DiscreteActions);
    }
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = act[0];
        var rotateAxis = act[1];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                break;

        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        rBody.AddForce(dirToGo * forceMultiplier, ForceMode.VelocityChange);
    }

    /// <summary>
    /// �ش� �Լ��� �������� Ȥ�� ��Ģ���ִ� �ڵ����� ���۽�Ű�� ���� �Լ�
    /// </summary>
    /// <param name="actionsOut"></param>

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut.Clear();
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }

        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}