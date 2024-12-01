using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    private GameManager gameManager;
    private Vector2Int gridSize;
    private Grid fullMap;
    private Tilemap obstacleTilemap;
    private HashSet<Vector2Int> obstaclesPos;


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

        // Get all obstacles positions
        this.obstaclesPos = new HashSet<Vector2Int>();
        BoundsInt bounds = this.obstacleTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (this.obstacleTilemap.HasTile(pos))
            {
                this.obstaclesPos.Add(new Vector2Int(pos.x, pos.y));
            }
        }
        // print obstacles positions
        // foreach (var pos in this.obstaclesPos)
        // {
        //     Debug.Log("Obstacle at " + pos);
        // }
        

        // draw path
        var self = this.gameManager.GetPositionOnMap(this.transform.position);
        var exit = this.gameManager.GetExitPositions()[0];
        // Debug.Log("Finding path from " + self + " to " + exit);
        var path = FindPath(self, exit);  
        this.DebugDrawPath(path);
    }


    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        // Open and closed lists
        var openList = new PriorityQueue<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start);
        Node goalNode = new Node(goal);

        openList.Enqueue(startNode, startNode.FCost);

        while (openList.Any())
        {
            Node currentNode = openList.Dequeue();

            // Goal reached
            if (currentNode.Position == goal)
                return RetracePath(startNode, currentNode);

            closedList.Add(currentNode);

            foreach (Vector2Int neighbor in GetNeighbors(currentNode.Position))
            {
                if (!IsWalkable(neighbor) || closedList.Any(n => n.Position == neighbor))
                    continue;

                int newMovementCost = currentNode.GCost + GetDistance(currentNode.Position, neighbor);

                Node neighborNode = null;
                if (openList.Contains(new Node(neighbor)))
                {
                    neighborNode = openList.Find(n => n.Position == neighbor);
                }

                if (neighborNode == null || newMovementCost < neighborNode.GCost)
                {
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighbor);
                        openList.Enqueue(neighborNode, neighborNode.FCost);
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

    private void DebugDrawPath(List<Vector2Int> path)
    {
        Assert.IsNotNull(path, "Path is null!");
        foreach (var position in path)
        {
            Debug.DrawLine(
                new Vector3(position.x, position.y, 0), 
                new Vector3(position.x + 1, position.y + 1, 0), 
                Color.green, 10f);
        }
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

        var (limitMin, limitMax) = this.gameManager.GetMapLimits();

        var res = neighbors.Where(
            n => 
                n.x >= limitMin.x && n.x < limitMax.x && 
                n.y >= limitMin.y && n.y < limitMax.y
        );
        // foreach (var neighbor in res)
        // {
        //     Debug.Log("\tFound Neighbor: " + neighbor);
        // }
        return res;
    }

    private bool IsWalkable(Vector2Int position)
    {
        return !this.obstaclesPos.Contains(position);
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

        public override bool Equals(object obj)
        {
            return obj is Node node &&
                   Position.Equals(node.Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }

    private class PriorityQueue<T>
    {
        private List<(T Item, int Priority)> elements = new List<(T, int)>();

        public void Enqueue(T item, int priority) => elements.Add((item, priority));
        public T Dequeue()
        {
            var bestIndex = elements.Select((e, i) => (e, i)).OrderBy(e => e.e.Priority).First().i;
            var bestItem = elements[bestIndex].Item;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
        public bool Any() => elements.Count > 0;

        public bool Contains(T item) => elements.Any(e => e.Item.Equals(item));

        public T Find(Predicate<T> match) => elements.Find(e => match(e.Item)).Item;
    }

}
