using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private List<EscaperAgent> escapers;
    private List<FinderAgent> finders;
    private Grid map;
    private Tilemap exit;


    public void Awake()
    {
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

    public void EscaperWin()
    {
        Debug.Log("Escaper has won the game!");

        // Notify EscaperAgent
        foreach (var escaper in escapers)
        {
            escaper.Win();
        }

        // Notify FinderAgent
        foreach (var finder in finders)
        {
            finder.Lose();
        }
    }

    public void FinderWin()
    {
        Debug.Log("Finder has won the game!");

        // Notify EscaperAgent
        foreach (var escaper in escapers)
        {
            escaper.Lose();
        }

        // Notify FinderAgent
        foreach (var finder in finders)
        {
            finder.Win();
        }
    }

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
                    Vector3Int posOnMap = this.map.WorldToCell(this.exit.CellToWorld(cellPos));
                    exitPositions.Add(new Vector2Int(posOnMap.x, posOnMap.y));
                }
            }
        }

        return exitPositions.ToArray();
    }


    ///// Getters and Setters /////
    public Grid Map
    {
        get { return this.map; }
    }
}
