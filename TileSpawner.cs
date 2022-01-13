using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public class Tile
    {
        GameManager gameManager;

        public GameObject tileObject;
        public TileManager tileManager;

        [Space(10)]
        [Header("Values")]
        public Vector3 position; //world position
        public bool startTile; //is start tile?
        public GameObject tileType; //tile prefab
        public int tileSize;

        [Space(10)]
        [Header("Tile Grid Placement")]
        public Vector2 gridPoint;
        public Tile leftNeighbor;
        public Tile rightNeighbor;
        public Tile bottomNeighbor;
        public Tile topNeighbor;

        public Tile(Vector2 tileGridPoint, GameObject type, int i_tileSize, bool start = false)
        {
            gridPoint = tileGridPoint;
            position = new Vector3(tileGridPoint.x * i_tileSize, -1, tileGridPoint.y * i_tileSize); //world space is grid point * tileSize
            tileType = type;
            startTile = start;
            tileSize = i_tileSize; // tile.tileSize = input tile size

            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

            //spawn new tile
            tileObject = Instantiate(tileType, position, Quaternion.identity); //create object
            tileObject.transform.localScale = new Vector3(i_tileSize, tileObject.transform.localScale.y, i_tileSize); //set scale based off tileSize
            tileObject.transform.parent = gameManager.tileParent; //set parent
            tileObject.name = type.name + "(" + (int)tileGridPoint.x + "," + (int)tileGridPoint.y + ")"; //name is type && grid coord
            
            
            //get tile manager
            tileManager = tileObject.GetComponent<TileManager>();

            //set start tile variable
            if (startTile)
            {
                tileManager.startTile = true;
                gameManager.startTile = tileManager;
            }
            else { tileManager.startTile = false; }

        }
    }


    // ====== TILE SPAWNER CLASS ===============================================================================

    public int tileSize = 30;
    public List<GameObject> tileTypes = new List<GameObject>();
    Vector3 spawnGroundPos;

    [Space(10)]
    [Header("Tile Grid")]
    Tile startTile;
    List<Tile> tileList = new List<Tile>();



    void Awake()
    {
        //Create Start Tile
        startTile = CreateTile(Vector2.zero, tileTypes[0], tileSize, true);
    }


    public Tile CreateTile(Vector2 tileGridPoint, GameObject type, int tileSize, bool start = false)
    {
        Tile newTile = new Tile(tileGridPoint, type, tileSize, start);

        newTile.tileManager.thisTile = newTile; //set thisTile == to created tile
        tileList.Add(newTile); //place tile in grid list
        return newTile;
    }

    public Tile CreateHorizontalNeighbor(Tile tile, int direction)
    {
        //set left neighbor
        if (direction == -1)
        {
            //if left neighbor not set....
            if (tile.leftNeighbor == null)
            {
                //create left neighbor of given tile
                tile.leftNeighbor = CreateTile(new Vector2(tile.gridPoint.x + direction, tile.gridPoint.y), tileTypes[0], tileSize);

                //set the right neighbor of created tile to given tile
                tile.leftNeighbor.rightNeighbor = tile;

                return tile.leftNeighbor;
            }
            else
            {
                print(tile.tileObject.name + " already has left neighbor");

                return null;
            }
        }
        //set right neighbor
        else if (direction == 1)
        {
            if (tile.rightNeighbor == null)
            {
                //create right neighbor of given tile
                tile.rightNeighbor = CreateTile(new Vector2(tile.gridPoint.x + direction, tile.gridPoint.y), tileTypes[0], tileSize);

                //set the left neighbor of created tile to given tile
                tile.rightNeighbor.leftNeighbor = tile;

                return tile.rightNeighbor;

            }
            else
            {
                print(tile.tileObject.name + " already has right neighbor");

                return null;
            }
        }

        return null;
    }

    public Tile CreateVerticalNeighbor(Tile tile, int direction)
    {
        //set bottom neighbor
        if (direction == -1)
        {
            //if left neighbor not set....
            if (tile.topNeighbor == null)
            {
                //create top neighbor of given tile
                tile.topNeighbor = CreateTile(new Vector2(tile.gridPoint.x, tile.gridPoint.y + direction), tileTypes[0], tileSize);

                //set the bottom neighbor of created tile to given tile
                tile.topNeighbor.bottomNeighbor = tile;

                return tile.topNeighbor;
            }
            else
            {
                print(tile.tileObject.name + " already has bottom neighbor");

                return null;

            }
        }
        //set right neighbor
        else if (direction == 1)
        {
            if (tile.bottomNeighbor == null)
            {
                //create right neighbor of given tile
                tile.bottomNeighbor = CreateTile(new Vector2(tile.gridPoint.x, tile.gridPoint.y + direction), tileTypes[0], tileSize);

                //set the left neighbor of created tile to given tile
                tile.bottomNeighbor.topNeighbor = tile;

                return tile.bottomNeighbor;
            }
            else
            {
                print(tile.tileObject.name + " already has top neighbor");

                return null;
            }
        }

        return null;
    }

    /*
    public bool IsGridPointAvailable(int x, int y)
    {

    }
    */

        /*
    public void PrintTileGrid()
    {
        //print(tileGrid)
    }
    */

}
