using UnityEngine;

public interface IMovementInput
{
    MovementInput.KeyMoveType keyMoveType { get; set; }

    public MovementState HandleInput();
}
