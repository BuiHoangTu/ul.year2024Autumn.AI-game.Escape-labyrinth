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


    void Awake()
    {
        Debug.Log("Registering starting position" + this.transform.position);
        this.startingPosition = this.transform.position;
        this.gameManager = this.GetComponentInParent<GameManager>();
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
        // Reset position
        Debug.Log("Resetting position" + this.startingPosition);
        this.transform.position = this.startingPosition;

        // Use movement if not set
        if (this.heuristicsMoveType == KeyMoveType.NONE)
        {
            this.heuristicsMoveType = this.characterMovement.keyMoveType;
        }

        // Stop external inputs if in training mode
        this.characterMovement.keyMoveType = KeyMoveType.NONE;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // self position
        var posOnMap = GetTilePosition();
        sensor.AddObservation(posOnMap);

        // self turning angle
        sensor.AddObservation(this.transform.rotation.eulerAngles.z);

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
        EndEpisode();
    }

    public void Lose()
    {
        AddReward(Rewards.LOSE);  // Evaluate the progress when losing
        EndEpisode();
    }

    // Find position on tile map
    public Vector2Int GetTilePosition()
    {
        var position = gameManager.Map.WorldToCell(this.transform.position);
        return new Vector2Int(position.x, position.y);
    }
}