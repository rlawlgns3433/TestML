using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ShootAgent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    private void ResetAgentPosition()
    {

    }

    private void MLShoot()
    {

    }

    private void MoveForward()
    {

    }

    private void MoveBackwawrd()
    {

    }

    private void RotateLeft()
    {

    }

    private void RotateRight()
    {

    }



}
