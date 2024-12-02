using UnityEngine;

public class MovementFSM
{
    public MovementState currentState { get; private set; }
    public float headingAngle
    {
        get => this._headingAngle;
        set
        {
            this._headingAngle = value;
            if (this._headingAngle < 0)
            {
                this._headingAngle += 360;
            }
            if (this._headingAngle >= 360)
            {
                this._headingAngle -= 360;
            }
        }
    }


    private float _headingAngle;


    public MovementFSM(float headingAngle)
    {
        this.currentState = MovementState.IDLE;
        this.headingAngle = headingAngle;
    }

    public bool TurnLeft()
    {
        if (
            this.currentState == MovementState.IDLE
            || this.currentState == MovementState.TURN_RIGHT
            || this.currentState == MovementState.MOVE_FORWARD
            || this.currentState == MovementState.MOVE_BACKWARD
        )
        {

            this.currentState = MovementState.TURN_LEFT;
            return true;
        }
        return false;

    }

    public bool TurnRight()
    {
        if (
            this.currentState == MovementState.IDLE
            || this.currentState == MovementState.TURN_LEFT
            || this.currentState == MovementState.MOVE_FORWARD
            || this.currentState == MovementState.MOVE_BACKWARD
        )
        {
            this.currentState = MovementState.TURN_RIGHT;
            return true;
        }
        return false;
    }

    public bool MoveForward()
    {
        if (
            this.currentState == MovementState.IDLE
            || this.currentState == MovementState.TURN_LEFT
            || this.currentState == MovementState.TURN_RIGHT
            || this.currentState == MovementState.MOVE_BACKWARD
        )
        {
            this.currentState = MovementState.MOVE_FORWARD;
            return true;
        }
        return false;
    }

    public bool MoveBackward()
    {
        if (
            this.currentState == MovementState.IDLE
            || this.currentState == MovementState.TURN_LEFT
            || this.currentState == MovementState.TURN_RIGHT
            || this.currentState == MovementState.MOVE_FORWARD
        )
        {
            this.currentState = MovementState.MOVE_BACKWARD;
            return true;
        }
        return false;
    }

    public bool Burst()
    {
        if (this.currentState == MovementState.MOVE_FORWARD)
        {
            this.currentState = MovementState.BURST_FORWARD;
            return true;
        }
        if (this.currentState == MovementState.MOVE_BACKWARD)
        {
            this.currentState = MovementState.BURST_BACKWARD;
            return true;
        }

        return false;
    }

    public bool Stop()
    {
        if (
            this.currentState == MovementState.TURN_LEFT
            || this.currentState == MovementState.TURN_RIGHT
            || this.currentState == MovementState.MOVE_FORWARD
            || this.currentState == MovementState.MOVE_BACKWARD
            || this.currentState == MovementState.BURST_FORWARD
            || this.currentState == MovementState.BURST_BACKWARD
        )
        {
            this.currentState = MovementState.IDLE;
            return true;
        }
        return false;
    }
}
