using System;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MapMemory : MonoBehaviour, ISensor
{
    public string sensorName = "MapMemory";
    public Vector2Int mapSize = new(100, 100);


    private GameManager gameManager;
    private MapItem[,] mapMemory;
    private readonly HashSet<GameObject> staticObjects = new();
    private Dictionary<int, Vector2Int> dynamicObjects = new();


    public void Start()
    {
        this.Reset();
    }

    public bool AddObstacle(GameObject obj)
    {
        return this.AddStaticObject(obj, MapItem.OSTACLE);
    }
    public bool AddStaticObject(GameObject obj, MapItem objType)
    {
        bool adding = this.staticObjects.Add(obj);
        if (adding)
        {
            Vector2Int cellPos = this.gameManager.GetPositionOnMap(obj.transform.position);
            this.mapMemory[cellPos.x, cellPos.y] = this.AddMapItem(this.mapMemory[cellPos.x, cellPos.y], objType);
        }
        return adding;
    }

    public void AddEnemy(GameObject obj)
    {
        this.AddDynamicObject(obj, MapItem.ENEMY);
    }
    public void AddDynamicObject(GameObject obj, MapItem objType)
    {
        int id = obj.GetInstanceID();
        if (this.dynamicObjects.ContainsKey(id))
        {
            var lastPos = this.dynamicObjects[id];
            var currPos = this.gameManager.GetPositionOnMap(obj.transform.position);

            if (lastPos == currPos)
                return;

            var lastState = this.mapMemory[lastPos.x, lastPos.y];
            this.mapMemory[lastPos.x, lastPos.y] = this.RemoveMapItem(lastState, objType);
            this.dynamicObjects[id] = currPos;
            this.mapMemory[currPos.x, currPos.y] = this.AddMapItem(this.mapMemory[currPos.x, currPos.y], objType);
        }
        else
        {
            Vector2Int currPos = this.gameManager.GetPositionOnMap(obj.transform.position);
            this.dynamicObjects.Add(id, currPos);
            this.mapMemory[currPos.x, currPos.y] = this.AddMapItem(this.mapMemory[currPos.x, currPos.y], objType);
        }
    }


    ///// ISensor /////
    public ObservationSpec GetObservationSpec()
    {
        return ObservationSpec.Vector(this.mapSize.x * this.mapSize.y * Enum.GetValues(typeof(MapItem)).Length);
    }

    public int Write(ObservationWriter writer)
    {
        var oneHotLength = Enum.GetValues(typeof(MapItem)).Length;

        for (int x = 0; x < this.mapSize.x; x++)
        {
            for (int y = 0; y < this.mapSize.y; y++)
            {
                for (int i = 0; i < oneHotLength; i++)
                {
                    writer[x * this.mapSize.y * oneHotLength + y * oneHotLength + i] = this.mapMemory[x, y] == (MapItem)i ? 1 : 0;
                }
            }
        }

        return this.mapSize.x * this.mapSize.y * oneHotLength;
    }

    public byte[] GetCompressedObservation()
    {
        int totalCells = this.mapSize.x * this.mapSize.y;
        byte[] compressedObservation = new byte[totalCells];

        // flatten 2D array into 1D
        for (int x = 0; x < this.mapSize.x; x++)
        {
            for (int y = 0; y < this.mapSize.y; y++)
            {
                // Use a more explicit flattening method
                int flattenedIndex = y * this.mapSize.x + x;
                compressedObservation[flattenedIndex] = (byte)this.mapMemory[x, y];
            }
        }

        return compressedObservation;
    }

    public CompressionSpec GetCompressionSpec()
    {
        // Specify the compression type if needed
        return CompressionSpec.Default();
    }

    public string GetName()
    {
        return this.sensorName;
    }

    public void Reset()
    {
        this.mapMemory = new MapItem[this.mapSize.x, this.mapSize.y];

        this.gameManager = this.GetComponentInParent<GameManager>();

        for (int x = 0; x < this.mapSize.x; x++)
        {
            for (int y = 0; y < this.mapSize.y; y++)
            {
                this.mapMemory[x, y] = MapItem.EMPTY;
            }
        }

        this.staticObjects.Clear();
        this.dynamicObjects.Clear();
    }

    public void Update()
    {
        return;
    }


    ///// Items on map /////
    public enum MapItem
    {
        EMPTY = 0,
        VISITED = 1,
        ENEMY = 2,
        ENEMY_AND_VISITED = 3,
        OSTACLE = 4,
    }

    private MapItem RemoveMapItem(MapItem item, MapItem toRemove)
    {
        if (item == MapItem.ENEMY_AND_VISITED)
        {
            return toRemove == MapItem.ENEMY ? MapItem.VISITED : MapItem.ENEMY;
        }

        // add more composite items here

        return MapItem.EMPTY;
    }

    private MapItem AddMapItem(MapItem item, MapItem toAdd)
    {
        var composite1 = MapItem.VISITED;
        var composite2 = MapItem.ENEMY;
        var fullComposite = MapItem.ENEMY_AND_VISITED;
        if ((item == composite1 && toAdd == composite2) || (item == composite2 && toAdd == composite1))
        {
            return fullComposite;
        }

        // add more composite items here

        return toAdd;
    }
}
