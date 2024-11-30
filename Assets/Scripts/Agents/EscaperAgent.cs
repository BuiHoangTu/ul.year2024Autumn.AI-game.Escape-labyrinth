using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class EscaperAgent : BaseAgent
{
    protected override string TargetTag => "Exit";

    protected override float SeeingTargetReward => Rewards.SEEING_EXIT;

    protected override float DistanceToTargetReward => Rewards.DISTANCE_TO_EXIT;

    protected override string SmartEnemyTag => "Finder";

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize()
    {
        var behaviorParams = this.GetComponent<BehaviorParameters>();
        behaviorParams.TeamId = (int)Team.Escaper;

        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }


    protected override float CalculateDistanceTargetScore()
    {
        // var exits = this.gameManager.GetExitPositions();
        // float minDistance = Vector2.Distance(this.GetTilePosition(), exits[0]);
        // for (int i = 1; i < exits.Length; i++)
        // {
        //     float distance = Vector2.Distance(this.GetTilePosition(), exits[i]);
        //     if (distance < minDistance)
        //     {
        //         minDistance = distance;
        //     }
        // }
        // return minDistance;

        return 0;
    }
}