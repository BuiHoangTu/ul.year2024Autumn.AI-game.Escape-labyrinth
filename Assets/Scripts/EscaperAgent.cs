using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static MovementInput;

public class EscaperAgent : Agent
{
    public static EscaperAgent INSTANCE;


    private CharacterMovement characterMovement;
    private MovementInput heuristicsMove;


    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Debug.LogWarning("EscaperAgent instance is already set!");
            Destroy(this);
        }
    }

    public override void Initialize()
    {
        this.characterMovement = this.GetComponent<CharacterMovement>();

        if (this.characterMovement == null)
        {
            Debug.LogError("CharacterMove component is not set!");
        }
    }

    public override void OnEpisodeBegin()
    {
        this.heuristicsMove = new MovementInput
        {
            keyMoveType = this.characterMovement.keyMoveType
        };

        // Stop external inputs if in training mode
        if (Academy.Instance.IsCommunicatorOn)
        {
            this.characterMovement.keyMoveType = KeyMoveType.NONE;
        }
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

        Debug.Log("isBurst: " + burst + ", moving: " + moving + ", turning: " + turning);

        this.characterMovement.Bursting = burst;
        this.characterMovement.Move = moving;
        this.characterMovement.Turn = turning;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
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