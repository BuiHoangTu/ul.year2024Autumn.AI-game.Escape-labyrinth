using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static MovementInput;
using Unity.MLAgents.Policies;

public class EscaperAgent : Agent
{
    [Header("Movement Settings")]
    [Tooltip("The type of movement to use for the agent. Infers from the CharacterMovement component if not set.")]
    public KeyMoveType heuristicsMoveType = KeyMoveType.NONE;


    private Vector3 startingPosition;
    private CharacterMovement characterMovement;
    private readonly MovementInput heuristicsMove = new();
    private GameManager gameManager;
    private RayPerceptionSensorComponent2D visionSensor;
    private float lastDistanceExitScore;
    private MapMemory mapMemory;


    void Awake()
    {
        Debug.Log("Registering starting position" + this.transform.position);
        this.startingPosition = this.transform.position;
        this.gameManager = this.GetComponentInParent<GameManager>();

    }

    void Start()
    {
        this.visionSensor = this.GetComponentInChildren<RayPerceptionSensorComponent2D>();
        if (this.visionSensor == null)
        {
            Debug.LogError("RayPerceptionSensorComponent2D not found!");
        }

        this.mapMemory = this.GetComponent<MapMemory>();
    }

    public override void Initialize()
    {
        var behaviorParams = this.GetComponent<BehaviorParameters>();
        behaviorParams.TeamId = (int)Team.Escaper;

        this.characterMovement = this.GetComponent<CharacterMovement>();

        if (this.characterMovement == null)
        {
            Debug.LogError("CharacterMove component is not set!");
        }
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");

        // Use movement if not set
        if (this.heuristicsMoveType == KeyMoveType.NONE)
        {
            this.heuristicsMoveType = this.characterMovement.keyMoveType;
        }

        // Stop external inputs if in training mode
        this.characterMovement.keyMoveType = KeyMoveType.NONE;

        this.lastDistanceExitScore = CalculateDistanceExitScore();
    }

    void FixedUpdate()
    {
        ////// Reward for seeing the exit //////
        // Iterate over all ray outputs (mid -> right -> left -> right -> left -> ...)
        var rayOutputs = RayPerceptionSensor.Perceive(this.visionSensor.GetRayPerceptionInput()).RayOutputs;
        foreach (var rayOutput in rayOutputs)
        {
            GameObject hitObject = rayOutput.HitGameObject;
            if (hitObject == null) continue;

            if (hitObject.CompareTag("Exit"))
            {
                this.AddReward(Rewards.SEEING_EXIT * Time.fixedDeltaTime);
                break;
            }
        }


        ///// Map Memory /////
        foreach (var rayOutput in rayOutputs)
        {
            GameObject hitObject = rayOutput.HitGameObject;
            if (hitObject == null) continue;

            if (hitObject.CompareTag("Obstacle"))
            {
                this.mapMemory.AddObstacle(hitObject);
            }
            else if (hitObject.CompareTag("Finder"))
            {
                this.mapMemory.AddEnemy(hitObject);
            }
        }
        this.mapMemory.AddStaticObject(this.gameObject, MapMemory.MapItem.VISITED);


        ////// Reward for reaching one of the exit //////
        var distanceExitScore = CalculateDistanceExitScore();
        var distanceExitScoreDelta = this.lastDistanceExitScore - distanceExitScore;
        this.lastDistanceExitScore = distanceExitScore;
        this.AddReward(distanceExitScoreDelta * Rewards.DISTANCE_TO_EXIT * Time.fixedDeltaTime);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // self position
        var posOnMap = GetTilePosition();
        sensor.AddObservation(posOnMap);

        // self turning angle
        sensor.AddObservation(this.transform.rotation.eulerAngles.z / 360);

        // self burst energy
        sensor.AddObservation(this.characterMovement.BurstEnergyPercentage);

        // exits position
        var exits = gameManager.GetExitPositions();
        if (exits.Length != 3)
        {
            Debug.LogError("There should be 3 exits on the map!");
        }
        sensor.AddObservation(exits[0]);
        sensor.AddObservation(exits[1]);
        sensor.AddObservation(exits[2]);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        bool burst = actions.DiscreteActions[0] == 1;
        MovingType moving = (MovingType)actions.DiscreteActions[1];
        TurningType turning = (TurningType)actions.DiscreteActions[2];

        // Debug.Log("isBurst: " + burst + ", moving: " + moving + ", turning: " + turning);

        this.characterMovement.Bursting = burst;
        this.characterMovement.Move = moving;
        this.characterMovement.Turn = turning;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // check for changes on the fly
        this.heuristicsMove.keyMoveType = this.heuristicsMoveType;

        var actions = actionsOut.DiscreteActions;
        actions[0] = this.heuristicsMove.GetBurstInput() ? 1 : 0;
        actions[1] = (int)this.heuristicsMove.GetMovementInput();
        actions[2] = (int)this.heuristicsMove.GetRotationInput();
    }

    public void Win()
    {
        SetReward(Rewards.WIN);  // Win is just win
        // reduce the reward by the time it took to win
        AddReward(- Rewards.PROLONG_MATCH * gameManager.TimeMatch / gameManager.timeMatchMax);  
        EndEpisode();
    }

    public void Lose()
    {
        AddReward(Rewards.LOSE);  // Evaluate the progress when losing
        // add reward for surviving
        AddReward(Rewards.PROLONG_MATCH * gameManager.TimeMatch / gameManager.timeMatchMax);
        EndEpisode();
    }

    public void ResetGameState()
    {
        this.transform.position = this.startingPosition;
        EndEpisode();
    }


    /// <summary>
    /// Get the current tile position on the map
    /// </summary>
    private Vector2Int GetTilePosition()
    {
        return gameManager.GetPositionOnMap(this.transform.position);
    }

    private float CalculateDistanceExitScore()
    {
        var exits = gameManager.GetExitPositions();
        float minDistance = Vector2.Distance(this.GetTilePosition(), exits[0]);
        for (int i = 1; i < exits.Length; i++)
        {
            float distance = Vector2.Distance(this.GetTilePosition(), exits[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }
        return minDistance;
    }
}