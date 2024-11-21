using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class EscaperAgent : Agent
{
    public static EscaperAgent INSTANCE;


    private CharacterMove characterMove;


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
        this.characterMove = this.GetComponent<CharacterMove>();

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
            this.characterMove.keyMoveType = CharacterMove.KeyMoveType.NONE;
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        bool isBurst = actions.DiscreteActions[0] == 1;
        Moving moving = (Moving)actions.DiscreteActions[1];
        Turning turning = (Turning)actions.DiscreteActions[2];

        switch (moving)
        {
            case Moving.FORWARD:
                this.characterMove.Move(true);
                break;
            case Moving.BACKWARD:
                this.characterMove.Move(false);
                break;
            case Moving.STOP:
                // Do nothing
                break;
        }

        switch (turning)
        {
            case Turning.LEFT:
                this.characterMove.Rotate(true);
                break;
            case Turning.RIGHT:
                this.characterMove.Rotate(false);
                break;
            case Turning.STOP:
                // Do nothing
                break;
        }

        Debug.Log(actions.ContinuousActions.Length);
    }

    public void Win()
    {
        AddReward(1.0f);
        EndEpisode();
    }


    /*************** Support enums ***************/
    public enum Moving {
        STOP,
        FORWARD,
        BACKWARD
    }
    public enum Turning {
        STOP,
        LEFT,
        RIGHT
    }
}