using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    private GameManager gameManager;
    private Vector2Int gridSize;
    private Grid fullMap;
    private Tilemap obstacleTilemap;


    private void Awake()
    {
        this.gameManager = this.GetComponentInParent<GameManager>();
        if (this.gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }
    }

    private void Start()
    {
        var (min, max) = this.gameManager.GetMapLimits();
        this.gridSize = new Vector2Int(max.x - min.x, max.y - min.y);

        this.fullMap = this.gameManager.Map;
        if (this.fullMap == null)
        {
            Debug.LogError("Full Map not found!");
        }

        this.obstacleTilemap = this.fullMap.transform.Find("Obstacles").GetComponent<Tilemap>();
        if (this.obstacleTilemap == null)
        {
            Debug.LogError("Obstacle Tilemap not found!");
        }
    }


    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        // Open and closed lists
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start);
        Node goalNode = new Node(goal);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.OrderBy(n => n.FCost).First();

            // Goal reached
            if (currentNode.Position == goal)
                return RetracePath(startNode, currentNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Vector2Int neighbor in GetNeighbors(currentNode.Position))
            {
                if (!IsWalkable(neighbor) || closedList.Any(n => n.Position == neighbor))
                    continue;

                int newMovementCost = currentNode.GCost + GetDistance(currentNode.Position, neighbor);

                Node neighborNode = openList.FirstOrDefault(n => n.Position == neighbor);
                if (neighborNode == null || newMovementCost < neighborNode.GCost)
                {
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighbor);
                        openList.Add(neighborNode);
                    }
                    neighborNode.GCost = newMovementCost;
                    neighborNode.HCost = GetDistance(neighbor, goal);
                    neighborNode.Parent = currentNode;
                }
            }
        }

        return null; // No path found
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int position)
    {
        var neighbors = new List<Vector2Int>
        {
            new(position.x + 1, position.y),
            new(position.x - 1, position.y),
            new(position.x, position.y + 1),
            new(position.x, position.y - 1)
        };

        return neighbors.Where(n => n.x >= 0 && n.x < this.gridSize.x && n.y >= 0 && n.y < this.gridSize.y);

    }

    private bool IsWalkable(Vector2Int position)
    {
        return this.obstacleTilemap.GetTile(new Vector3Int(position.x, position.y, 0)) == null;
    }

    private int GetDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance
    }

    private class Node
    {
        public Vector2Int Position;
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;
        public Node Parent;

        public Node(Vector2Int position)
        {
            Position = position;
        }
    }
}
