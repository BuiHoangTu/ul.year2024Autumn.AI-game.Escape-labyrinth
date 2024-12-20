using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public float timeMatchMax = 600.0f;


    private float timeMatch;
    private List<EscaperAgent> escapers;
    private List<FinderAgent> finders;
    private Grid map;
    private Tilemap exit;


    public void Awake()
    {
        this.timeMatch = 0;

        this.escapers = new List<EscaperAgent>();
        this.finders = new List<FinderAgent>();

        // Find all Escapers and Finders that are chidren of this
        var searchFinder = GetComponentsInChildren<FinderAgent>();
        Debug.Log("Found " + searchFinder.Length + " FinderAgents");

        var searchEscaper = GetComponentsInChildren<EscaperAgent>();
        Debug.Log("Found " + searchEscaper.Length + " EscaperAgents");

        // Add them to the list
        this.finders.AddRange(searchFinder);
        this.escapers.AddRange(searchEscaper);


        // Find the grid
        this.map = GetComponentInChildren<Grid>();
        this.exit = this.map.transform.Find("Exit").GetComponent<Tilemap>();

        var exitPos = this.GetExitPositions();
        Debug.Log("Exits: " + exitPos.Length + " " + string.Join(", ", exitPos));
    }

    public void FixedUpdate()
    {
        this.timeMatch += Time.fixedDeltaTime;

        if (this.timeMatch >= this.timeMatchMax)
        {
            Debug.Log("Time is up!");
            this.Tie();
        }
    }

    public void EscaperWin()
    {
        Debug.Log("Escaper has won the game!");

        // // Notify EscaperAgent
        // foreach (var escaper in escapers)
        // {
        //     escaper.Win();
        // }

        // // Notify FinderAgent
        // foreach (var finder in finders)
        // {
        //     finder.Lose();
        // }

        GameEnded();
    }

    public void FinderWin()
    {
        Debug.Log("Finder has won the game!");

        // // Notify EscaperAgent
        // foreach (var escaper in escapers)
        // {
        //     escaper.Lose();
        // }

        // // Notify FinderAgent
        // foreach (var finder in finders)
        // {
        //     finder.Win();
        // }

        GameEnded();
    }

    public void Tie()
    {
        Debug.Log("It's a tie!");

        // // Notify EscaperAgent
        // foreach (var escaper in escapers)
        // {
        //     escaper.Lose();
        // }

        // // Notify FinderAgent
        // foreach (var finder in finders)
        // {
        //     finder.Lose();
        // }

        GameEnded();
    }

    /// <summary>
    /// Get the positions of the exit tiles on the map
    /// </summary>
    public Vector2Int[] GetExitPositions()
    {
        List<Vector2Int> exitPositions = new();

        BoundsInt bounds = this.exit.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new(x, y, 0);
                if (this.exit.HasTile(cellPos))
                {
                    exitPositions.Add(this.GetPositionOnMap(this.exit.CellToWorld(cellPos)));
                }
            }
        }

        return exitPositions.ToArray();
    }

    /// <summary>
    /// Get the position of the exit tile on the map
    /// </summary>
    public Vector2Int GetPositionOnMap(Vector2 posInWorld)
    {
        var pos = this.map.WorldToCell(posInWorld);

        return new Vector2Int(pos.x, pos.y);
    }

    /// <summary>
    /// Convert a position on the map to world position
    /// </summary>
    public Vector2 GetWorldPosition(Vector2Int posOnMap)
    {
        return this.map.CellToWorld(new Vector3Int(posOnMap.x, posOnMap.y, 0));
    }


    public (Vector2Int, Vector2Int) GetMapLimits()
    {
        var bounds = this.map.transform.Find("Obstacles").GetComponent<Tilemap>().cellBounds;
        return (new Vector2Int(bounds.xMin, bounds.yMin), new Vector2Int(bounds.xMax, bounds.yMax));
    }

    private void GameEnded()
    {
        this.timeMatch = 0;

        // // Reset all agents
        // foreach (var escaper in escapers)
        // {
        //     escaper.ResetGameState();
        // }

        // foreach (var finder in finders)
        // {
        //     finder.ResetGameState();
        // }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    ///// Getters and Setters /////
    public Grid Map
    {
        get { return this.map; }
    }

    public float TimeMatch
    {
        get { return this.timeMatch; }
    }
}
