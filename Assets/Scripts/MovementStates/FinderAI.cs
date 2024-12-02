using System.Collections.Generic;
using UnityEngine;


public class FinderAI: MonoBehaviour, IMovementInput
{
    public float MaxAngleDelta = 20.0f;
    public float RandomTurnChance = 50.0f;
    public FinderAI teammate;


    private PathFinder pathFinder;
    private List<Vector2Int> path;
    private int currentPathIndex;
    private FinderState fsmState;
    private MovementFSM movementFSM;
    private GameManager gameManager;


    void Start()
    {
        if (this.teammate == null)
        {
            Debug.LogError("Missing teammate!");
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
    }

    void Update()
    {
        // if (this.fsmState == FinderState.IDLE)
        // {
        //     // find the shortest path to the exit
        //     Vector2Int[] exits = this.gameManager.GetExitPositions();
        //     Vector2Int[][] solutions = new Vector2Int[exits.Length][];
        //     for (int i = 0; i < exits.Length; i++)
        //     {
        //         this.path = this.pathFinder.FindPath(exits[i]);
        //         solutions[i] = this.path.ToArray();
        //     }

        //     int shortestPathIndex = 0;
        //     int shortestPathLength = solutions[0].Length;
        //     for (int i = 1; i < solutions.Length; i++)
        //     {
        //         if (solutions[i].Length < shortestPathLength)
        //         {
        //             shortestPathIndex = i;
        //             shortestPathLength = solutions[i].Length;
        //         }
        //     }

        //     this.path = new List<Vector2Int>(solutions[shortestPathIndex]);
        //     this.currentPathIndex = 0;
        //     this.fsmState = FinderState.GUARDING_EXIT;
        // }
        // else if (this.fsmState == FinderState.GUARDING_EXIT)
        // {
        //     if (this.currentPathIndex < this.path.Count)
        //     {
        //         Vector2Int nextPos = this.path[this.currentPathIndex];
        //         Vector2Int currentPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);

        //         if (nextPos == currentPos)
        //         {
        //             this.currentPathIndex++;
        //         }
        //         else
        //         {
        //             Vector2Int direction = nextPos - currentPos;
        //             float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //             this.movementFSM.SetAngle(angle);
        //         }
        //     }
        //     else
        //     {
        //         this.fsmState = FinderState.CHANGE_GUARDING_EXIT;
        //     }
        // }
        // else if (this.fsmState == FinderState.CHANGE_GUARDING_EXIT)
        // {
        //     // find the shortest path to the exit
        //     Vector2Int[] exits = this.gameManager.GetExitPositions();
        //     Vector2Int[][] solutions = new Vector2Int[exits.Length][];
        //     for (int i = 0; i < exits.Length; i++)
        //     {
        //         this.path = this.pathFinder.FindPath(exits[i]);
        //         solutions[i] = this.path.ToArray();
        //     }

        //     int shortestPathIndex = 0;
        //     int

    }

    public MovementInput.KeyMoveType keyMoveType
    {
        get => MovementInput.KeyMoveType.NONE;
        set { }
    }

    public MovementState HandleInput()
    {
        

        return MovementState.IDLE;
    }



    ///// Finder states /////
    private enum FinderState
    {
        IDLE,
        GUARDING_EXIT,
        CHANGE_GUARDING_EXIT,
        CHASING_ESCAPER,
    }
}