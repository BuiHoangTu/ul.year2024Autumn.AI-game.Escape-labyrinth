using UnityEngine;

public class MovementInput : MonoBehaviour, IMovementInput
{
    public KeyMoveType keyMoveType = KeyMoveType.ARROR;

    public MovingType GetMovementInput()
    {
        return keyMoveType switch
        {
            KeyMoveType.ARROR => Input.GetKey(KeyCode.UpArrow) ? MovingType.FORWARD : Input.GetKey(KeyCode.DownArrow) ? MovingType.BACKWARD : MovingType.STOP,
            KeyMoveType.WASD => Input.GetKey(KeyCode.W) ? MovingType.FORWARD : Input.GetKey(KeyCode.S) ? MovingType.BACKWARD : MovingType.STOP,
            KeyMoveType.IJKL => Input.GetKey(KeyCode.I) ? MovingType.FORWARD : Input.GetKey(KeyCode.K) ? MovingType.BACKWARD : MovingType.STOP,
            _ => MovingType.STOP,
        };
    }

    public TurningType GetRotationInput()
    {
        return keyMoveType switch
        {
            KeyMoveType.ARROR => Input.GetKey(KeyCode.RightArrow) ? TurningType.RIGHT : Input.GetKey(KeyCode.LeftArrow) ? TurningType.LEFT : TurningType.STOP,
            KeyMoveType.WASD => Input.GetKey(KeyCode.D) ? TurningType.RIGHT : Input.GetKey(KeyCode.A) ? TurningType.LEFT : TurningType.STOP,
            KeyMoveType.IJKL => Input.GetKey(KeyCode.L) ? TurningType.RIGHT : Input.GetKey(KeyCode.J) ? TurningType.LEFT : TurningType.STOP,
            _ => TurningType.STOP,
        };
    }

    public bool GetBurstInput()
    {
        return keyMoveType switch
        {
            KeyMoveType.ARROR => Input.GetKey(KeyCode.Space),
            KeyMoveType.WASD => Input.GetKey(KeyCode.Z),
            KeyMoveType.IJKL => Input.GetKey(KeyCode.M),
            _ => false,
        };
    }

    public MovementState HandleInput()
    {
        var move = GetMovementInput();
        var turn = GetRotationInput();
        var burst = GetBurstInput();

        if (burst)
        {
            return move switch
            {
                MovingType.FORWARD => MovementState.BURST_FORWARD,
                MovingType.BACKWARD => MovementState.BURST_BACKWARD,
                _ => MovementState.IDLE
            };
        }

        if (move != MovingType.STOP)
        {
            return move switch
            {
                MovingType.FORWARD => MovementState.MOVE_FORWARD,
                MovingType.BACKWARD => MovementState.MOVE_BACKWARD,
                _ => MovementState.IDLE
            };
        }

        if (turn != TurningType.STOP)
        {
            return turn switch
            {
                TurningType.LEFT => MovementState.TURN_LEFT,
                TurningType.RIGHT => MovementState.TURN_RIGHT,
                _ => MovementState.IDLE
            };
        }

        return MovementState.IDLE;
    }

    KeyMoveType IMovementInput.keyMoveType { get => keyMoveType; set => keyMoveType = value; }

    /*************** Support enums ***************/
    public enum KeyMoveType
    {
        ARROR,
        WASD,
        IJKL,
        NONE
    }


}
