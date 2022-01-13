using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ER_TileSpawner : MonoBehaviour
{

    public GameManager gameManager;
    public TileSpawner tileSpawner;
    public PlayerTileMovement playerTileMovement;

    Vector2 playerCoord;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        tileSpawner = gameManager.tileSpawner;
        playerTileMovement = gameManager.playerTileMovement;
    }

    // Update is called once per frame
    void Update()
    {
        // just in case init doesnt happen fast enough
        if (tileSpawner == null || playerTileMovement == null)
        {
            tileSpawner = gameManager.tileSpawner;
            playerTileMovement = gameManager.playerTileMovement;
        }

        // get player coord
        playerCoord = playerTileMovement.playerCoordinate;
        TileSpawner.Tile currentTile = playerTileMovement.currTileManager.thisTile;

        // if player in top half of tile
        if (playerCoord.y > playerTileMovement.currTileManager.tileSize / 2)
        {
            tileSpawner.CreateVerticalNeighbor(currentTile, 1);
        }
        
    }
}
