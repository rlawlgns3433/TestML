using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ShootingAgent : Agent
{
    public Transform shootingPoint;
    public int minStepBetweenShots = 50;
    public int damage = 100;

    private bool shotAvailable = true;
    private int StepsUntilShotIsAvailable = 0;

    private Vector3 startingPosition;
    private Rigidbody Rb;

    private void Shoot()
    {
        if (!shotAvailable) return;

        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        var direction = transform.forward;
        Debug.Log("Shot");
        Debug.DrawRay(shootingPoint.position, direction * 200f, Color.green, 2f);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            hit.transform.GetComponent<Enemy>().GetShot(damage, this);
        }

        shotAvailable = false;
        StepsUntilShotIsAvailable = minStepBetweenShots;
    }

    private void FixedUpdate()
    {
        if(!shotAvailable)
        {
            StepsUntilShotIsAvailable--;

            if (StepsUntilShotIsAvailable <= 0)
                shotAvailable = true;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var vectorAction = actions.ContinuousActions;
        
        if(Mathf.RoundToInt(vectorAction[0]) >= 1)
        {
            Shoot();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    public override void Initialize()
    {
        startingPosition = transform.position;
        Rb = GetComponent<Rigidbody>();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.P) ? 1 : 0;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode Begin");
        transform.position = startingPosition;
        Rb.velocity = Vector3.zero;
        shotAvailable = true;
    }

    public void RegisterKill()
    {
        AddReward(1.0f);
        EndEpisode();
    }

}