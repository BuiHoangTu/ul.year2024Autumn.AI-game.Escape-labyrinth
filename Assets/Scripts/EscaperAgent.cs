using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static MovementController;

public class EscaperAgent : Agent
{
    public static EscaperAgent INSTANCE;


    private CharacterMovement characterMove;


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
        this.characterMove = this.GetComponent<CharacterMovement>();

        if (this.characterMove == null)
        {
            Debug.LogError("CharacterMove component is not set!");
        }
    }

    public override void OnEpisodeBegin()
    {

        // Stop external inputs if in training mode
        if (Academy.Instance.IsCommunicatorOn)
        {
            this.characterMove.keyMoveType = KeyMoveType.NONE;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add observation for the position of the agent
        sensor.AddObservation((Vector2)this.transform.position);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        bool isBurst = actions.DiscreteActions[0] == 1;
        MovingType moving = (MovingType)actions.DiscreteActions[1];
        TurningType turning = (TurningType)actions.DiscreteActions[2];

        Debug.Log("isBurst: " + isBurst + ", moving: " + moving + ", turning: " + turning);

        this.characterMove.Move = moving;
        this.characterMove.Turn = turning;
    }

    public void Win()
    {
        AddReward(1.0f);
        EndEpisode();
    }
}