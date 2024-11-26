using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MovementController;

public class CharacterMovement : MonoBehaviour
{
    private static readonly float MAX_BURST_ENERGY = 100.0f;


    public float speed = 5.0f;
    public float angularSpeed = 200.0f;
    public float maxBurstEnergy = MAX_BURST_ENERGY;
    public float burstMultiplier = 2.0f; 
    public float burstConsumptionRate = 20.0f;
    public float burstRechargeRate = 10.0f;
    public KeyMoveType keyMoveType = KeyMoveType.ARROR;


    private float burstEnergy;
    private bool isBursting;
    private bool needRechargeBurst;
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
        this.burstEnergy = this.maxBurstEnergy;
        this.isBursting = false;
        this.needRechargeBurst = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.movementController.keyMoveType = this.keyMoveType;

        if (this.keyMoveType != KeyMoveType.NONE)
        {
            this.moving = this.movementController.GetMovementInput();
            this.turning = this.movementController.GetRotationInput();
            this.isBursting = this.movementController.GetBurstInput();
        }

        this.BurstEvaluate();
        this.MoveEvaluate();
        this.RotateEvaluate();
    }

    private void MoveEvaluate()
    {
        float currentSpeed = moving switch
        {
            MovingType.FORWARD => this.speed,
            MovingType.BACKWARD => -this.speed,
            _ => 0
        };

        // Apply burst multiplier if bursting
        if (isBursting)
        {
            currentSpeed *= this.burstMultiplier;
        }

        this.rb.velocity = this.heading * currentSpeed;
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

    private void BurstEvaluate()
    {
        // Using Burst
        if (this.isBursting && this.moving != MovingType.STOP)
        {
            this.burstEnergy -= this.burstConsumptionRate * Time.deltaTime;
        }
        // Recharging Burst
        else
        {
            if (this.burstEnergy <= this.maxBurstEnergy * 1/2)
            {
                this.needRechargeBurst = true;
            }

            this.burstEnergy += this.burstRechargeRate * Time.deltaTime;
        }

        // Burst Energy bounds
        if (this.burstEnergy <= 0)
        {
            this.burstEnergy = 0;
            this.isBursting = false;
        }
        else if (this.burstEnergy >= this.maxBurstEnergy)
        {
            this.burstEnergy = this.maxBurstEnergy;
            this.needRechargeBurst = false;
        }
    }


    // Getters and Setters
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

    // public bool IsBursting
    // {
    //     get => this.isBursting;
    // }

    // public float BurstEnergyPercentage
    // {
    //     get => this.burstEnergy / this.maxBurstEnergy;
    // }
}
