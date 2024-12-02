using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MovementInput;

public class CharacterMovement : MonoBehaviour
{
    private static readonly float MAX_BURST_ENERGY = 100.0f;


    public float speed = 5.0f;
    public float angularSpeed = 50.0f;
    public float maxBurstEnergy = MAX_BURST_ENERGY;
    public float burstMultiplier = 1.5f;
    public float burstConsumptionRate = 20.0f;
    public float burstRechargeRate = 10.0f;
    public KeyMoveType keyMoveType = KeyMoveType.ARROR;


    private float burstEnergy;
    private bool needRechargeBurst;
    private IMovementInput movementController;
    private Vector3 heading;
    private Rigidbody2D rb;
    private MovementState moveState;
    private GameObject headingIndicator;
    private MovementFSM movementFSM;


    // Start is called before the first frame update
    void Start()
    {
        this.heading = this.transform.up;
        this.rb = this.GetComponent<Rigidbody2D>();
        this.headingIndicator = this.transform.Find("HeadingIndicator").gameObject;
        this.movementController.keyMoveType = this.keyMoveType;
        this.burstEnergy = this.maxBurstEnergy;
        this.needRechargeBurst = false;
        this.movementFSM = new();

        // Modify based on Tag
        if (this.CompareTag("Escaper"))
        {
            this.burstMultiplier = 1.8f;
        }
        else if (this.CompareTag("Finder"))
        {
            this.speed *= 1.1f;
            this.burstMultiplier = 1.3f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.movementController.keyMoveType = this.keyMoveType;

        if (this.keyMoveType != KeyMoveType.NONE)
        {
            this.moveState = this.movementController.HandleInput();
        }

        this.BurstEvaluate();
        this.MoveEvaluate();
        this.RotateEvaluate();
    }

    private void MoveEvaluate()
    {
        float currentSpeed = this.moveState switch
        {
            MovementState.MOVE_FORWARD => this.speed,
            MovementState.MOVE_BACKWARD => -this.speed,
            MovementState.BURST_FORWARD => this.speed * this.burstMultiplier,
            MovementState.BURST_BACKWARD => -this.speed * this.burstMultiplier,
            _ => 0
        };

        this.rb.velocity = this.heading * currentSpeed;
    }

    private void RotateEvaluate()
    {
        float angle = this.moveState switch
        {
            MovementState.TURN_RIGHT => -this.angularSpeed * Time.deltaTime,
            MovementState.TURN_LEFT => this.angularSpeed * Time.deltaTime,
            _ => 0
        };

        this.heading = Quaternion.Euler(0, 0, angle) * this.heading;
        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.heading);
    }

    private void BurstEvaluate()
    {
        // Using Burst
        if (this.moveState == MovementState.BURST_FORWARD || this.moveState == MovementState.BURST_BACKWARD)
        {
            this.burstEnergy -= this.burstConsumptionRate * Time.deltaTime;
            if (this.burstEnergy <= 0)
            {
                this.burstEnergy = 0;
                this.StopBurst();
            }
        }
        // Recharging Burst
        else
        {
            this.burstEnergy += this.burstRechargeRate * Time.deltaTime;

            if (this.burstEnergy >= this.maxBurstEnergy)
            {
                this.burstEnergy = this.maxBurstEnergy;
                this.needRechargeBurst = false;
            }
        }
    }

    private void StartBurst()
    {
        if (this.needRechargeBurst)
        {
            return;
        }
    }

    private void StopBurst()
    {
        if (this.burstEnergy <= this.maxBurstEnergy * 1 / 2)
        {
            this.needRechargeBurst = true;
        }
    }


    ///// Getters and Setters /////
    public MovingType Move
    {
        get => this.moveState switch
        {
            MovementState.MOVE_FORWARD => MovingType.FORWARD,
            MovementState.MOVE_BACKWARD => MovingType.BACKWARD,
            _ => MovingType.STOP
        };
        set => this.moveState = value switch
        {
            MovingType.FORWARD => MovementState.MOVE_FORWARD,
            MovingType.BACKWARD => MovementState.MOVE_BACKWARD,
            _ => MovementState.IDLE
        };
    }

    public TurningType Turn
    {
        get => this.moveState switch
        {
            MovementState.TURN_LEFT => TurningType.LEFT,
            MovementState.TURN_RIGHT => TurningType.RIGHT,
            _ => TurningType.STOP
        };
        set => this.moveState = value switch
        {
            TurningType.LEFT => MovementState.TURN_LEFT,
            TurningType.RIGHT => MovementState.TURN_RIGHT,
            _ => MovementState.IDLE
        };
    }

    public bool Bursting
    {
        get => this.moveState switch
        {
            MovementState.BURST_FORWARD => true,
            MovementState.BURST_BACKWARD => true,
            _ => false
        };
        // ignore set
        set { }
    }

    public float BurstEnergyPercentage
    {
        get => this.burstEnergy / this.maxBurstEnergy;
    }
}
