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


    private CharacterMovement characterMovement;
    private readonly MovementInput heuristicsMove = new();


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
        // Use movement if not set
        if (this.heuristicsMoveType == KeyMoveType.NONE) {
            this.heuristicsMoveType = this.characterMovement.keyMoveType;
        }

        // Stop external inputs if in training mode
        this.characterMovement.keyMoveType = KeyMoveType.NONE;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add observation for the position of the agent
        sensor.AddObservation((Vector2)this.transform.position);
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
        AddReward(1.0f);
        EndEpisode();
    }
}