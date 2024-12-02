using System.Collections.Generic;
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
    }

    void Update()
    {
        if (this.fsmState == EscaperState.IDLE)
        {
            // find the shortest path to the exit
            Vector2Int[] exits = this.gameManager.GetExitPositions();
            Vector2Int[][] solutions = new Vector2Int[exits.Length][];
            for (int i = 0; i < exits.Length; i++)
            {
                this.path = this.pathFinder.FindPath(exits[i]);
                solutions[i] = this.path.ToArray();
            }

            int shortestPathIndex = 0;
            int shortestPathLength = solutions[0].Length;
            for (int i = 1; i < solutions.Length; i++)
            {
                if (solutions[i].Length < shortestPathLength)
                {
                    shortestPathIndex = i;
                    shortestPathLength = solutions[i].Length;
                }
            }
            this.path = new List<Vector2Int>(solutions[shortestPathIndex]);
            this.currentPathIndex = 0;

            this.fsmState = EscaperState.TO_EXIT;

            this.pathFinder.DebugDrawPath(this.path);
        }
    }

    public MovementInput.KeyMoveType keyMoveType
    {
        get => MovementInput.KeyMoveType.NONE;
        set { }
    }

    public MovementState HandleInput()
    {
        if (this.fsmState == EscaperState.TO_EXIT)
        {
            return this.FollowPath();
        }
        


        return MovementState.IDLE;
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
            // targetAngle -= 90; // adjust angle to match the map direction

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
                if (deltaAngle < - 1/2 * this.MaxAngleDelta)
                    return MovementState.TURN_RIGHT;
                if (deltaAngle > 1/2 * this.MaxAngleDelta)
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



    ///// AI States
    private enum EscaperState
    {
        IDLE,
        TO_EXIT,
        FLEEING,
    }
}
