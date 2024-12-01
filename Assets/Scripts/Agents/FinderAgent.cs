using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System.Collections.Generic;

public class FinderAgent : BaseAgent
{
    public List<FinderAgent> teamMateAgents = new();


    private float distanceToTargetScore;

    protected override string TargetTag => "Escaper";

    protected override float SeeingTargetReward => Rewards.SEEING_ESCAPER;

    protected override float DistanceToTargetReward => Rewards.CHASING_ESCAPER;

    protected override string SmartEnemyTag => "Escaper";

    protected override void Awake()
    {
        base.Awake();
        
        // filter self from teamMates
        this.teamMateAgents = this.teamMateAgents.FindAll(agent => agent != this);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        this.distanceToTargetScore = 1;
        var rayOutputs = RayPerceptionSensor.Perceive(this.visionSensor.GetRayPerceptionInput()).RayOutputs;
        foreach (var rayOutput in rayOutputs)
        {
            GameObject hitObject = rayOutput.HitGameObject;
            if (hitObject == null) continue;

            if (hitObject.CompareTag(this.TargetTag))
            {
                this.distanceToTargetScore = rayOutput.HitFraction;
                break;
            }
        }
    }

    /// <summary>
    /// 12 observations
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        // Add teamMates states
        if (this.teamMateAgents.Count != 1)
            Debug.LogError("This FinderAgent implementation should have only one teamMate!");

        foreach (var teamMate in this.teamMateAgents)
        {
            // 2 observations
            sensor.AddObservation(teamMate.GetTilePosition());
        }
    }


    protected override float CalculateDistanceTargetScore()
    {
        return this.distanceToTargetScore;
    }
}
