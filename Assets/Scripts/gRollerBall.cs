using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
//mlAgent 사용시 포함해야 됨

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
        //새로운 애피소드 시작시, 다시 에이전트의 포지션의 초기화
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0) //에이전트가 floor 아래로 떨어진 경우 추가 초기화
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

        //타겟의 위치는 에피소드 시작시 랜덤하게 변경된다.
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
    /// 강화학습 프로그램에게 관측정보를 전달
    /// </summary>
    /// <param name="sensor"></param>

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    /// <summary>
    /// 강화학습을 위한, 강화학습을 통한 행동이 결정되는 곳
    /// 기존
    /// </summary>
    public float forceMultiplier = 4;
    //public override void OnActionReceived(ActionBuffers actionBuffers)
    //{
    //    if(transform.position.normalized.z == Target.transform.position.z)
    //    {
    //        Invoke("Shoot", 0.1f);
    //    }

    //    //학습을 위한, 학습된 정보를 해석하여 이동을 시킨다.

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

    //    //타겟을 찾을시 리워드점수를 주고, 에피소드를 종료시킨다.
    //    // Reached target
    //    if (distanceToTarget < 1.42f)
    //    {
    //        SetReward(1.0f);
    //        EndEpisode();
    //    }

    //    //판 아래로 떨어지면 학습이 종료된다.
    //    // Fell off platform
    //    //else if (this.transform.localPosition.y < 0)
    //    //{
    //    //    EndEpisode();
    //    //}
    //}


    /// <summary>
    /// RayAgents에서 가져온 함수
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

        //타겟을 찾을시 리워드점수를 주고, 에피소드를 종료시킨다.
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        transform.Rotate(rot, Time.deltaTime * turnSpeed);
        this.GetComponent<Rigidbody>().AddForce(dir * moveSpeed, ForceMode.VelocityChange);

    }

    //학습할 때 ON
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isCollid = true;
            SetReward(-1.0f); // 수정
            EndEpisode();
        }
        else if(collision.gameObject.CompareTag("Target"))
        {
            SetReward(+1.0f);
            EndEpisode();
        }
    }


    /// <summary>
    /// 해당 함수는 직접조작 혹은 규칙성있는 코딩으로 조작시키기 위한 함수
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
