using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MovementController;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float angularSpeed = 200.0f;
    public KeyMoveType keyMoveType = KeyMoveType.ARROR;

    private readonly MovementController movementController = new();
    private Vector3 heading;
    private Rigidbody2D rb;
    private MovingType moving;
    private TurningType turning;
    private GameObject headingIndicator;

    // Start is called before the first frame update
    void Start()
    {
        this.heading = Vector3.up;
        this.rb = this.GetComponent<Rigidbody2D>();
        this.headingIndicator = this.transform.Find("HeadingIndicator").gameObject;
        this.movementController.keyMoveType = this.keyMoveType;
    }

    // Update is called once per frame
    void Update()
    {
        this.movementController.keyMoveType = this.keyMoveType;

        if (this.keyMoveType != KeyMoveType.NONE)
        {
            this.moving = this.movementController.GetMovementInput();
            this.turning = this.movementController.GetRotationInput();
        }

        this.MoveEvaluate();
        this.RotateEvaluate();
    }

    private void MoveEvaluate()
    {
        float speed = moving switch
        {
            MovingType.FORWARD => this.speed,
            MovingType.BACKWARD => -this.speed,
            _ => 0
        };

        this.rb.velocity = this.heading * speed;
    }

    private void RotateEvaluate()
    {
        float angle = turning switch
        {
            TurningType.RIGHT => -this.angularSpeed * Time.deltaTime,
            TurningType.LEFT => this.angularSpeed * Time.deltaTime,
            _ => 0
        };

        this.heading = Quaternion.Euler(0, 0, angle) * this.heading;
        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.heading);
    }

    public MovingType Move
    {
        get => this.moving;
        set => this.moving = value;
    }

    public TurningType Turn
    {
        get => this.turning;
        set => this.turning = value;
    }
}
