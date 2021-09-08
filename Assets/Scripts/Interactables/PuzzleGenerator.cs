using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PuzzleGenerator : MonoBehaviour
{
    public int columns = 5;
    public int rows = 5;
    public float tileSize = 1;
    public int seed = 10;
    [Range(0, 1)]
    public float obstaclePercent;

    [Space]
    public GameObject playerPrefab;
    public GameObject exit;
    public GameObject[] blockTile;
    public GameObject[] outerBlockTile;
    public GameObject floorTile;
    public GameObject burstPrefab;
    public Transform puzzlePosition;

    GameObject exitObj;

    private Transform mapHolder;

    List<Coordinate> allTileCoords;
    Queue<Coordinate> shuffledTileCoords;
    Queue<Coordinate> shuffledOpenTileCoords;

    [HideInInspector]
    public Coordinate startingPosition;


    public int[,] grid;
    public int[,] pass;
    public int[,] stop;
    public int[,] walk;

    public int starThreshold = 8;

    public int currentDistance;
    Queue<Coordinate> coordQueue;
    List<Coordinate> starCoord;

    PuzzleController player;

    public bool hasComplete;

    public UnityEvent OnExitPuzzleEvent;
    public UnityEvent OnCompletePuzzleEvent;

    private void Awake()
    {
        
    }
    void Start()
    {
        //GameManager.instance.OnGameOver += OnGameOver;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Interact"))
        {
            OnExitPuzzleEvent?.Invoke();
        }
    }

    private void OnEnable()
    {
        startingPosition = GetRandomCoord();
        InitializeGrid();
        GenerateMap();
    }

    

    void InitializeGrid()
    {
        grid = new int[columns, rows];
        pass = new int[columns, rows];
        stop = new int[columns, rows];
        walk = new int[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                grid[i, j] = 0; //0: walkable, 1: wall
                pass[i, j] = 1000;
                stop[i, j] = 1000;
                walk[i, j] = 1000;
            }
        }
    }

    void RemoveDeadEnd()
    {
        for (int x = 0; x < 4; x++)
        {
            for (var i = 1; i < columns - 1; i++)
            {
                for (var j = 1; j < rows - 1; j++)
                {
                    if (grid[i, j] == 0 && grid[i - 1, j] + grid[i + 1, j] + grid[i, j + 1] + grid[i, j - 1] >= 3)
                    {
                        var _fixed = false;

                        while (_fixed == false)
                        {
                            switch (Random.Range(0, 4))
                            {
                                case 0:
                                    if (grid[i - 1, j] == 1 && i > 1)
                                    {
                                        _fixed = true;
                                        grid[i - 1, j] = 0;
                                    }
                                    break;
                                case 1:
                                    if (grid[i + 1, j] == 1 && i < columns - 2)
                                    {
                                        _fixed = true;
                                        grid[i + 1, j] = 0;
                                    }
                                    break;
                                case 2:
                                    if (grid[i, j - 1] == 1 && j > 1)
                                    {
                                        _fixed = true;
                                        grid[i, j - 1] = 0;
                                    }
                                    break;
                                case 3:
                                    if (grid[i, j + 1] == 1 && j < rows - 2)
                                    {
                                        _fixed = true;
                                        grid[i, j + 1] = 0;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
            }
        }

    }

    public Vector2 CoordToPosition(int x, int y)
    {
        return new Vector2(-columns / 2f + x + 0.5f, -rows / 2f + y + 0.5f) * tileSize; //Add 0.5f since we want the edge of the tile at the position        
    }

    public Vector2 CoordToWorldPosition(int x, int y)
    {
        return transform.TransformPoint(new Vector2(-columns / 2f + x + 0.5f, -rows / 2f + y + 0.5f) * tileSize);
    }

    public void GenerateMap()
    {
        string holderName = "Generated Puzzle";
        if (transform.Find(holderName))
        {
            Destroy(transform.Find(holderName).gameObject);
            DestroyImmediate(transform.Find(holderName).gameObject);
        }


        seed = Random.Range(1, 1000);
        InitializeGrid();

        allTileCoords = new List<Coordinate>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                allTileCoords.Add(new Coordinate(x, y));
            }
        }

        shuffledTileCoords = new Queue<Coordinate>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

        if (mapHolder != null)            
            Destroy(mapHolder.gameObject);

        mapHolder = new GameObject(holderName).transform;  //Set board holder to the tranform of the gameObject "Board"
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(100, 100, Camera.main.nearClipPlane));
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        mapHolder.parent = transform;
        mapHolder.transform.position = Vector3.zero;

        SpawnPlayer();


        //Spawning Obstacle
        bool[,] obstacleMap = new bool[(int)columns, (int)rows];
        int obstacleCount = (int)(columns * rows * obstaclePercent); //get a percentage of obstacle from number of tiles
        int currentObstacleCount = 0;
        //Make a copy of allTileCoords List
        List<Coordinate> allOpenCoords = new List<Coordinate>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coordinate randomCoord = GetRandomCoord();

            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != startingPosition && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                grid[randomCoord.x, randomCoord.y] = 1;

                //Remove the obstacle coords from the open tiles list
                allOpenCoords.Remove(randomCoord);
            }
            //When can't generate new obstacle
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--; //Reduce currentObstacleCount back
            }
        }

        shuffledOpenTileCoords = new Queue<Coordinate>(Utility.ShuffleArray(allOpenCoords.ToArray(), seed));

        RemoveDeadEnd();
        SpawnBlocks();

        bool success = MakeDistance();
        if (!success)
        {
            if (transform.Find(holderName))
            {
                Destroy(transform.Find(holderName).gameObject);
            }
            GenerateMap();
        }

        mapHolder.position = transform.position;
    }

    void SpawnPlayer()
    {
        player = Instantiate(playerPrefab).GetComponent<PuzzleController>();
        player.transform.parent = mapHolder;
        player.map = this;
        startingPosition = GetRandomCoord();
        player.SetPosition(startingPosition.x, startingPosition.y);
    }

    void SpawnBlocks()
    {
        //Spawn Outer Wall
        for (int x = -1; x < columns + 1; x++) //looping from -1 to columns/rows +1 to create the outer wall
        {
            for (int y = -1; y < rows + 1; y++)
            {
                if (x == -1 || x == columns || y == -1 || y == rows)     //if position is outer wall choose outer wall tile           
                {
                    GameObject instance = null;
                    //if (x == -1 && y >= 0 && y < rows)
                    //{
                    //    if (grid[0, y] == 0)
                    //    {
                    //        instance = Instantiate(blockTile[Random.Range(0, blockTile.Length)], CoordToPosition(x, y), Quaternion.identity) as GameObject;
                    //        instance.transform.SetParent(mapHolder);
                    //    }
                    //}
                    //else if (x == columns && y >= 0 && y < rows)
                    //{
                    //    if (grid[columns - 1, y] == 0)
                    //    {
                    //        instance = Instantiate(blockTile[Random.Range(0, blockTile.Length)], CoordToPosition(x, y), Quaternion.identity) as GameObject;
                    //        instance.transform.SetParent(mapHolder);
                    //    }
                    //}
                    //else if (y == -1 && x >= 0 && x < columns)
                    //{
                    //    if (grid[x, 0] == 0)
                    //    {
                    //        instance = Instantiate(blockTile[Random.Range(0, blockTile.Length)], CoordToPosition(x, y), Quaternion.identity) as GameObject;
                    //        instance.transform.SetParent(mapHolder);
                    //    }
                    //}
                    //else if (y == rows && x >= 0 && x < columns)
                    //{
                    //    if (grid[x, rows - 1] == 0)
                    //    {
                    //        instance = Instantiate(blockTile[Random.Range(0, blockTile.Length)], CoordToPosition(x, y), Quaternion.identity) as GameObject;
                    //        instance.transform.SetParent(mapHolder);
                    //    }
                    //}

                    //else
                    //    instance = Instantiate(outerBlockTile[Random.Range(0, outerBlockTile.Length)], CoordToPosition(x, y), Quaternion.identity) as GameObject;
                    instance = Instantiate(blockTile[Random.Range(0, blockTile.Length)], CoordToPosition(x, y), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(mapHolder);
                    if (instance)
                        instance.transform.SetParent(mapHolder);

                }
            }
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (grid[i, j] == 1)
                {
                    GameObject instance;
                    if ((i == 0 || grid[i - 1, j] == 1) && (i == columns - 1 || grid[i + 1, j] == 1)
                        && (j == 0 || grid[i, j - 1] == 1) && (j == rows - 1 || grid[i, j + 1] == 1))
                        instance = Instantiate(outerBlockTile[Random.Range(0, outerBlockTile.Length)], CoordToPosition(i, j), Quaternion.identity) as GameObject;


                    else
                        instance = Instantiate(blockTile[Random.Range(0, blockTile.Length)], CoordToPosition(i, j), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(mapHolder);
                }
                else if (grid[i, j] == 0)
                {
                    GameObject instance = Instantiate(floorTile, CoordToPosition(i, j), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(mapHolder);
                }
            }
        }
    }
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coordinate> queue = new Queue<Coordinate>();
        queue.Enqueue(startingPosition);
        mapFlags[startingPosition.x, startingPosition.y] = true;

        int accessibleTileCount = 1;

        //Flood-fill

        while (queue.Count > 0)
        {
            Coordinate tile = queue.Dequeue();

            //Loop through the 4 adjacent neighboring tiles (horizontal/vertical)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;

                    //Check horizontal/vertical adjacent tile
                    if (x == 0 || y == 0)
                    {
                        //Check if the tile is in the map (when the selected tile on edge of the map)
                        //(0,0) is the bottom left coord of the map
                        if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1)) //Guaranteed the tile is in the map
                        {
                            //Check if tile hasn't already been checked and not an obstacle tile
                            if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coordinate(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(columns * rows - currentObstacleCount);

        return targetAccessibleTileCount == accessibleTileCount;
    }

    public Coordinate GetRandomCoord()
    {
        if (shuffledTileCoords == null)
            return startingPosition;

        Coordinate randomCoord = shuffledTileCoords.Dequeue(); //Get the first coordinate out of the queue
        shuffledTileCoords.Enqueue(randomCoord); // Add it back to end of queue
        return randomCoord;
    }

    public Coordinate GetRandomOpenTile()
    {
        Coordinate randomCoord = shuffledOpenTileCoords.Dequeue();
        while (randomCoord == startingPosition)
        {
            shuffledOpenTileCoords.Enqueue(randomCoord);
            randomCoord = shuffledOpenTileCoords.Dequeue();
        }
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    bool MakeDistance()
    {
        pass[startingPosition.x, startingPosition.y] = 0;
        stop[startingPosition.x, startingPosition.y] = 0;

        coordQueue = new Queue<Coordinate>();
        coordQueue.Enqueue(startingPosition);

        starCoord = new List<Coordinate>();

        int loop = 0;

        while (coordQueue.Count != 0)
        {
            loop++;
            var element = coordQueue.Dequeue();
            currentDistance = stop[element.x, element.y];

            CheckLine(0, 1, element.x, element.y);
            CheckLine(0, -1, element.x, element.y);
            CheckLine(1, 0, element.x, element.y);
            CheckLine(-1, 0, element.x, element.y);
        }
        coordQueue.Clear();

        if (starCoord.Count > 0)
        {
            int index = Random.Range(0, starCoord.Count);
            exitObj = Instantiate(exit, CoordToPosition(starCoord[index].x, starCoord[index].y), Quaternion.identity);
            exitObj.transform.SetParent(mapHolder);
            return true;
        }

        return false;


    }

    void CheckLine(int x, int y, int start_x, int start_y)
    {
        var x_delta = x;
        var y_delta = y;

        var _x = start_x + x_delta;
        var _y = start_y + y_delta;

        if (_x > columns - 1 || _x < 0 || _y > rows - 1 || _y < 0)
            return;

        while (grid[_x, _y] == 0)
        {
            pass[_x, _y] = Mathf.Min(pass[_x, _y], currentDistance + 1);

            //Potential star location
            if (pass[_x, _y] > starThreshold)
            {
                starCoord.Add(new Coordinate(_x, _y));
            }

            _x += x_delta;
            _y += y_delta;

            if (_x > columns - 1 || _x < 0 || _y > rows - 1 || _y < 0)
                break;
        }

        _x -= x_delta;
        _y -= y_delta;

        if (stop[_x, _y] == 1000)
        {
            coordQueue.Enqueue(new Coordinate(_x, _y));
            stop[_x, _y] = currentDistance + 1;
        }
    }

    public void OnCompletePuzzle()
    {
        hasComplete = true;
        OnCompletePuzzleEvent?.Invoke();
        Instantiate(burstPrefab, puzzlePosition.position, Quaternion.identity);
        puzzlePosition.gameObject.SetActive(false);
        var sfx = AppRoot.Instance.GetService<SfxController>();
        sfx.Play("fx-complete");
    }

    #region Coordinate
    [System.Serializable]
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Coordinate coordinate)
            {
                return this == coordinate;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return new { x, y }.GetHashCode();
        }
    }
    #endregion
}
