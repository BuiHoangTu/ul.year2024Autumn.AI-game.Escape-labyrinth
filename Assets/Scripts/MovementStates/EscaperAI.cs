using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class EscaperAI : MonoBehaviour, IMovementInput
{
    public float MaxAngleDelta = 20.0f;
    public float RandomTurnChance = 50.0f;


    private PathFinder pathFinder;
    private List<Vector2Int> path;
    private int currentPathIndex;
    private EscaperState fsmState;
    private MovementFSM movementFSM;
    private GameManager gameManager;

    // detect dangers
    private RayPerceptionSensorComponent2D visionSensor;
    private Queue<Vector2Int> lastPositions;
    private Vector2Int? lastSafePlace;



    void Start()
    {
        this.pathFinder = this.GetComponent<PathFinder>();
        if (this.pathFinder == null)
        {
            Debug.LogError("PathFinder not found!");
        }

        this.path = new List<Vector2Int>();
        this.currentPathIndex = 0;

        this.fsmState = EscaperState.IDLE;

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

        this.visionSensor = this.GetComponentInChildren<RayPerceptionSensorComponent2D>();
        if (this.visionSensor == null)
        {
            Debug.LogError("RayPerceptionSensorComponent2D not found!");
        }

        this.lastPositions = new Queue<Vector2Int>(5);
        var currentPos = this.gameManager.GetPositionOnMap(this.transform.position);
        for (int i = 0; i < 5; i++)
        {
            this.lastPositions.Enqueue(currentPos);
        }
    }

    void Update()
    {
        if (this.fsmState == EscaperState.IDLE)
        {
            this.path = this.WayToExit().ToList();
            this.currentPathIndex = 0;

            this.fsmState = EscaperState.TO_EXIT;

            this.pathFinder.DebugDrawPath(this.path);
        }

        // scan for enemies
        var danger = this.SeeingDanger();
        if (danger != null)
        {
            // reset path
            this.pathFinder.SetDangerPosition(
                danger.GetInstanceID(),
                this.gameManager.GetPositionOnMap(danger.transform.position)
            );
            this.path = this.WayToExit().ToList();
            this.currentPathIndex = 0;
            
            // check if enemy is seeing me 
            Vector3 directionSeeingMe = this.transform.position - danger.transform.position;
            float itAngle = Mathf.Atan2(directionSeeingMe.y, directionSeeingMe.x) * Mathf.Rad2Deg - danger.transform.eulerAngles.z;
            itAngle -= 90; // adjust angle to match the map direction
            itAngle = Mathf.Abs(itAngle);

            if (itAngle > 180)
            {
                itAngle = 360 - itAngle;
            }
            // it sees me, flee
            if (itAngle <= 45)
            {
                this.fsmState = EscaperState.FLEEING;
            }

        }

        // update last positions
        var currentPos = this.gameManager.GetPositionOnMap(this.transform.position);
        var lastPos = this.lastPositions.ToArray().Last();
        if (currentPos != lastPos)
        {
            this.lastPositions.Enqueue(currentPos);
            this.lastPositions.Dequeue();
        }
    }

    public MovementInput.KeyMoveType keyMoveType
    {
        get => MovementInput.KeyMoveType.NONE;
        set { }
    }

    public MovementState HandleInput()
    {
        return this.fsmState switch
        {
            EscaperState.IDLE => MovementState.IDLE,
            EscaperState.TO_EXIT => FollowPath(),
            EscaperState.FLEEING => Flee(),
            _ => MovementState.IDLE,
        };
    }


    private MovementState FollowPath()
    {
        if (this.currentPathIndex < this.path.Count)
        {
            Vector2 nextPos = this.gameManager.GetWorldPosition(this.path[this.currentPathIndex]);
            nextPos += new Vector2(0.5f, 0.5f); // center of the tile

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
            if (UnityEngine.Random.Range(0, 100) < this.RandomTurnChance)
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
            return MovementState.MOVE_FORWARD;
        }

        // reached the target
        this.fsmState = EscaperState.IDLE;

        return MovementState.IDLE;
    }

    private GameObject SeeingDanger()
    {
        var rayOutputs = RayPerceptionSensor.Perceive(this.visionSensor.GetRayPerceptionInput()).RayOutputs;
        foreach (var rayOutput in rayOutputs)
        {
            GameObject hitObject = rayOutput.HitGameObject;
            if (hitObject == null) continue;

            if (hitObject.CompareTag("Finder"))
            {
                return hitObject;
            }
        }

        return null;
    }

    private MovementState Flee()
    {
        if (lastSafePlace == null)
        {
            lastSafePlace = this.lastPositions.Dequeue();
            Debug.Log("Fleeing to: " + lastSafePlace);
        }

        if (lastSafePlace == null)
        {
            Debug.LogError("Error no safe place found!");
        }

        Vector2 nextPos = this.gameManager.GetWorldPosition((Vector2Int)lastSafePlace);
        nextPos += new Vector2(0.5f, 0.5f); // center of the tile

        Vector2 currPos = this.transform.position;

        Vector2 deltaPos = nextPos - currPos;

        if ((deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y) < 0.1)
        {
            this.lastPositions.Clear();
            // reached the safe place
            for (int i = 0; i < 5; i++)
            {
                this.lastPositions.Enqueue((Vector2Int)lastSafePlace);
            }
            lastSafePlace = null;
            this.fsmState = EscaperState.IDLE;
        }

        float targetAngle = Mathf.Atan2(deltaPos.y, deltaPos.x) * Mathf.Rad2Deg;
        targetAngle -= 90; // adjust angle to match the map direction

        // delta angle > angle threshold, need to rotate
        float deltaAngle = Mathf.DeltaAngle(this.movementFSM.headingAngle - 180, targetAngle);
        if (Mathf.Abs(deltaAngle) > this.MaxAngleDelta)
        {
            Debug.Log("Self: " + currPos + " Target: " + nextPos + " Target Angle: " + targetAngle + " Current Angle: " + this.movementFSM.headingAngle + " Delta Angle: " + deltaAngle);

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
        if (UnityEngine.Random.Range(0, 100) < this.RandomTurnChance)
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

        return MovementState.BURST_BACKWARD;
    }

    private Vector2Int[] WayToExit()
    {
        // find the shortest path to the exit
            Vector2Int[] exits = this.gameManager.GetExitPositions();
            Vector2Int[][] solutions = new Vector2Int[exits.Length][];
            for (int i = 0; i < exits.Length; i++)
            {
                this.path = this.pathFinder.FindPath(exits[i]);
                if (this.path == null)
                    solutions[i] = null;
                else
                    solutions[i] = this.path.ToArray();
            }

            int shortestPathIndex = -1;
            int shortestPathLength = int.MaxValue;
            for (int i = 0; i < solutions.Length; i++)
            {
                if (solutions[i] == null) continue;
                if (solutions[i].Length < shortestPathLength)
                {
                    shortestPathIndex = i;
                    shortestPathLength = solutions[i].Length;
                }
            }
            return solutions[shortestPathIndex];
            
    }


    ///// AI States
    private enum EscaperState
    {
        IDLE,
        TO_EXIT,
        FLEEING,
    }
}
