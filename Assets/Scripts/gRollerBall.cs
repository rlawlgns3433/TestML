using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
//mlAgent ���� �����ؾ� ��

public class gRollerBall : Agent
{ 
    Rigidbody rBody;
    public Transform Target;
    public Transform obstacle;
    public float rotate_val = 20;
    public float moveSpeed = 1.5f;
    public float turnSpeed = 200.0f;
    bool isCollid = false;

    //public void Shoot()
    //{
    //    GameObject bullet = Instantiate(bulletPrefab);
    //    bullet.transform.position = transform.position;
    //    var bulletDirec = transform.forward;
    //    bullet.GetComponent<Rigidbody>().AddForce(bulletDirec, ForceMode.Impulse);
    //}

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {

        rBody.velocity = rBody.angularVelocity = Vector3.zero;
        //���ο� ���Ǽҵ� ���۽�, �ٽ� ������Ʈ�� �������� �ʱ�ȭ
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0) //������Ʈ�� floor �Ʒ��� ������ ��� �߰� �ʱ�ȭ
        {
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
        obstacle.localPosition = new Vector3(Random.value * 8 - 3,
                                           0.5f,
                                           Random.value * 8 - 3);
        //obstacle.localEulerAngles = new Vector3(0.0f,
        //                                   Random.value * 90,
        //                                   0.0f);
    }

    /// <summary>
    /// ��ȭ�н� ���α׷����� ���������� ����
    /// </summary>
    /// <param name="sensor"></param>

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    /// <summary>
    /// ��ȭ�н��� ����, ��ȭ�н��� ���� �ൿ�� �����Ǵ� ��
    /// ����
    /// </summary>
    public float forceMultiplier = 4;
    //public override void OnActionReceived(ActionBuffers actionBuffers)
    //{
    //    if(transform.position.normalized.z == Target.transform.position.z)
    //    {
    //        Invoke("Shoot", 0.1f);
    //    }

    //    //�н��� ����, �н��� ������ �ؼ��Ͽ� �̵��� ��Ų��.

    //    // Actions, size = 2
    //    Vector3 controlSignal = Vector3.zero;
    //    controlSignal.x = actionBuffers.ContinuousActions[0];
    //    controlSignal.z = actionBuffers.ContinuousActions[1];

    //    //transform.position += (controlSignal * forceMultiplier * Time.deltaTime);
    //    rBody.AddForce(controlSignal * forceMultiplier);

    //    Vector3 EulerAngleVelocity = new Vector3(0, Random.value * 90 - Random.value * 90, 0);
    //    Quaternion deltaRotation = Quaternion.Euler(EulerAngleVelocity * Time.deltaTime * Random.value * rotate_val);


    //    rBody.MoveRotation(rBody.rotation * deltaRotation);


    //    // Rewards
    //    float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

    //    //Ÿ���� ã���� ������������ �ְ�, ���Ǽҵ带 �����Ų��.
    //    // Reached target
    //    if (distanceToTarget < 1.42f)
    //    {
    //        SetReward(1.0f);
    //        EndEpisode();
    //    }

    //    //�� �Ʒ��� �������� �н��� ����ȴ�.
    //    // Fell off platform
    //    //else if (this.transform.localPosition.y < 0)
    //    //{
    //    //    EndEpisode();
    //    //}
    //}


    /// <summary>
    /// RayAgents���� ������ �Լ�
    /// </summary>

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch (action[0])
        {
            case 1: dir = transform.forward; break;
            case 2: dir = -transform.forward; break;
        }

        switch (action[1])
        {
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        //Ÿ���� ã���� ������������ �ְ�, ���Ǽҵ带 �����Ų��.
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        transform.Rotate(rot, Time.deltaTime * turnSpeed);
        this.GetComponent<Rigidbody>().AddForce(dir * moveSpeed, ForceMode.VelocityChange);

    }

    //�н��� �� ON
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isCollid = true;
            SetReward(-1.0f); // ����
            EndEpisode();
        }
        else if(collision.gameObject.CompareTag("Target"))
        {
            SetReward(+1.0f);
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
        var discreteActionsOut = actionsOut.DiscreteActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

    }
}
