using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class FinderAI : MonoBehaviour, IMovementInput
{
    public float MaxAngleDelta = 20.0f;
    public float RandomTurnChance = 50.0f;
    public float ChangingPatrolPosTime = 20;
    public FinderAI teammate;


    private PathFinder pathFinder;
    private List<Vector2Int> path;
    private int currentPathIndex;
    private FinderState fsmState;
    private MovementFSM movementFSM;
    private GameManager gameManager;

    // for choosing patrol position
    private Vector2Int[] exits;
    private int guardingExitIndex;
    private Vector2Int targetPos;

    // for changing patrol position
    private float timeToChangePatrolPos;

    // for turn 1 round
    private float observingAngleLeft;
    private float lastAngle;
    private MovementState observingTurnDirection;

    // for vision 
    private RayPerceptionSensorComponent2D visionSensor;



    void Start()
    {
        if (this.teammate == null)
        {
            Debug.LogError("Missing teammate!");
        }
        if (this.teammate == this)
        {
            Debug.LogError("Teammate cannot be itself!");
        }

        this.pathFinder = this.GetComponent<PathFinder>();
        if (this.pathFinder == null)
        {
            Debug.LogError("PathFinder not found!");
        }

        this.path = new List<Vector2Int>();
        this.currentPathIndex = 0;

        this.fsmState = FinderState.IDLE;

        this.movementFSM = this.GetComponent<CharacterMovement>().movementFSM;
        if (this.movementFSM == null)
        {
            Debug.LogError("MovementFSM not found!");
        }

        this.gameManager = this.GetComponentInParent<GameManager>();
        if (this.gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }

        this.exits = this.gameManager.GetExitPositions();

        while (true)
        {
            this.guardingExitIndex = Random.Range(0, this.exits.Length);
            if (this.guardingExitIndex != teammate.guardingExitIndex)
            {
                break;
            }
        }

        this.timeToChangePatrolPos = this.ChangingPatrolPosTime;

        this.observingAngleLeft = 0;
        this.observingTurnDirection = MovementState.IDLE;

        this.visionSensor = this.GetComponentInChildren<RayPerceptionSensorComponent2D>();
    }

    void Update()
    {
        // change patrol position
        this.timeToChangePatrolPos -= Time.deltaTime;
        if (this.timeToChangePatrolPos <= 0)
        {
            this.timeToChangePatrolPos = this.ChangingPatrolPosTime;

            while (true)
            {
                this.guardingExitIndex = Random.Range(0, this.exits.Length);
                if (this.guardingExitIndex != teammate.guardingExitIndex)
                {
                    break;
                }
            }
        }

        if (this.fsmState == FinderState.IDLE)
        {
            this.targetPos = this.RandomPatrolPos(this.guardingExitIndex);
            this.path = this.pathFinder.FindPath(this.targetPos);
            this.currentPathIndex = 0;

            this.fsmState = FinderState.PATROLLING;
        }

        // vision
        var rayOutputs = RayPerceptionSensor.Perceive(this.visionSensor.GetRayPerceptionInput()).RayOutputs;
        foreach (var rayOutput in rayOutputs)
        {
            GameObject hitObject = rayOutput.HitGameObject;
            if (hitObject == null) continue;

            if (hitObject.CompareTag("Escaper"))
            {
                var targetPos = this.gameManager.GetPositionOnMap(hitObject.transform.position);

                this.teammate.Alert(targetPos);

                if (targetPos != this.targetPos)
                {
                    this.path = this.pathFinder.FindPath(this.gameManager.GetPositionOnMap(hitObject.transform.position));
                    this.currentPathIndex = 0;
                }

                this.fsmState = FinderState.CHASING_ESCAPER;
                break;
            }
        }
    }

    private Vector2Int RandomPatrolPos(int index)
    {
        var positions = this.pathFinder.FindNearbyPositions(this.exits[index], 5);

        return positions[Random.Range(0, positions.Count)];
    }

    public MovementInput.KeyMoveType keyMoveType
    {
        get => MovementInput.KeyMoveType.NONE;
        set { }
    }

    public MovementState HandleInput()
    {
        switch (this.fsmState)
        {
            case FinderState.PATROLLING:
                return this.FollowPath(FinderState.OBSERVING, false);

            case FinderState.OBSERVING:
                return this.Turn1Round();

            case FinderState.CHASING_ESCAPER:
                return this.FollowPath(FinderState.OBSERVING, true);

            case FinderState.IDLE:
            default:
                return MovementState.IDLE;
        }
    }

    private MovementState FollowPath(FinderState endState, bool isChasing)
    {
        if (this.currentPathIndex < this.path.Count)
        {
            Vector2 nextPos = this.gameManager.GetWorldPosition(this.path[this.currentPathIndex]);
            nextPos += new Vector2(0.5f, 0.5f); // center of the cell

            Vector2 currPos = this.transform.position;

            Vector2 deltaPos = nextPos - currPos;

            if ((deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y) < 0.1)
            {
                this.currentPathIndex++;
                if (this.currentPathIndex < this.path.Count)
                    nextPos = this.path[this.currentPathIndex];
            }

            float targetAngle = Mathf.Atan2(deltaPos.y, deltaPos.x) * Mathf.Rad2Deg;
            targetAngle -= 90; // adjust angle to match the map direction

            // delta angle > angle threshold, need to rotate
            float deltaAngle = Mathf.DeltaAngle(this.movementFSM.headingAngle, targetAngle);
            if (Mathf.Abs(deltaAngle) > this.MaxAngleDelta)
            {
                // Debug.Log("Self: " + currPos + " Target: " + nextPos + " Target Angle: " + targetAngle + " Current Angle: " + this.movementFSM.headingAngle + " Delta Angle: " + deltaAngle);

                if (deltaAngle < 0)
                {
                    return MovementState.TURN_RIGHT;
                }
                else
                {
                    return MovementState.TURN_LEFT;
                }
            }

            // random chance to turn
            if (Random.Range(0, 100) < this.RandomTurnChance)
            {
                if (deltaAngle < -1 / 2 * this.MaxAngleDelta)
                    return MovementState.TURN_RIGHT;
                if (deltaAngle > 1 / 2 * this.MaxAngleDelta)
                    return MovementState.TURN_LEFT;

                if (deltaAngle < 0)
                    return MovementState.TURN_LEFT;
                if (deltaAngle > 0)
                    return MovementState.TURN_RIGHT;
            }

            // move forward
            if (isChasing)
            {
                return MovementState.BURST_FORWARD;
            }
            return MovementState.MOVE_FORWARD;
        }

        // reached the target
        this.fsmState = endState;

        return MovementState.IDLE;
    }

    private MovementState Turn1Round()
    {
        if (this.observingTurnDirection == MovementState.IDLE)
        {
            this.observingAngleLeft = 360;
            this.lastAngle = this.movementFSM.headingAngle;
            this.observingTurnDirection = Random.Range(0, 2) == 0 ? MovementState.TURN_LEFT : MovementState.TURN_RIGHT;

            return this.observingTurnDirection;
        }

        float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(this.lastAngle, this.movementFSM.headingAngle));
        this.observingAngleLeft -= deltaAngle;

        if (this.observingAngleLeft <= 0)
        {
            this.observingTurnDirection = MovementState.IDLE;
            this.fsmState = FinderState.IDLE;

            return MovementState.IDLE;
        }

        return this.observingTurnDirection;
    }

    public void Alert(Vector2Int escaperPos)
    {
        if (escaperPos != this.targetPos)
        {
            this.targetPos = escaperPos;
            this.path = this.pathFinder.FindPath(escaperPos);
            this.currentPathIndex = 0;
        }

        this.fsmState = FinderState.CHASING_ESCAPER;
    }


    ///// Finder states /////
    private enum FinderState
    {
        IDLE,
        PATROLLING,
        OBSERVING,
        CHASING_ESCAPER,
    }
}