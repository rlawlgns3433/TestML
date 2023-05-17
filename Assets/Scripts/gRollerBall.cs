using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
//mlAgent ���� �����ؾ� ��

public class gRollerBall : Agent
{
    bool isCollid = false;
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void OnEpisodeBegin()
    {
        //���ο� ���Ǽҵ� ���۽�, �ٽ� ������Ʈ�� �������� �ʱ�ȭ
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0) //������Ʈ�� floor �Ʒ��� ������ ��� �߰� �ʱ�ȭ
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        if (isCollid)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 1.0f, 0);
            isCollid = false;
        }

        //Ÿ���� ��ġ�� ���Ǽҵ� ���۽� �����ϰ� ����ȴ�.
        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,
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
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //�н��� ����, �н��� ������ �ؼ��Ͽ� �̵��� ��Ų��.

        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        //Ÿ���� ������ ������������ �ְ�, ���Ǽҵ带 �����Ų��.
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        //�� �Ʒ��� �������� �н��� ����ȴ�.
        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    //�н��� �� ON
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isCollid = true;
            SetReward(-1.0f);
            EndEpisode();
        }
    }


    /// <summary>
    /// �ش� �Լ��� �������� Ȥ�� ��Ģ���ִ� �ڵ����� ���۽�Ű�� ���� �Լ�
    /// </summary>
    /// <param name="actionsOut"></param>

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
