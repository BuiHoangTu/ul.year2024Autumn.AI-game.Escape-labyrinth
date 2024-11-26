using UnityEngine;

public class MovementController
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


    /*************** Support enums ***************/
    public enum KeyMoveType
    {
        ARROR,
        WASD,
        IJKL,
        NONE
    }

    public enum MovingType
    {
        STOP,
        FORWARD,
        BACKWARD
    }
    public enum TurningType
    {
        STOP,
        LEFT,
        RIGHT
    }
}
