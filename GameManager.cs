using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    


    public GameObject playerPrefab;
    public TileSpawner tileSpawner;
    public PlayerTileMovement playerTileMovement;

    public GameObject player;
    public TileManager startTile;
    public Transform tileParent;

    public bool playerPlaced;


    private void Start()
    {
        tileSpawner = GetComponentInChildren<TileSpawner>();

        PlayerInit();
    }

    public void PlayerInit()
    {
        player = Instantiate(playerPrefab, startTile.originWorldPos, Quaternion.identity);
        player.name = "Player";
        player.transform.parent = transform;
        playerTileMovement = player.GetComponent<PlayerTileMovement>();

        playerTileMovement.currTileManager = startTile; //current tile is the start tile
        playerTileMovement.playerCoordinate = playerTileMovement.currTileManager.originCoord; //starts at center


        playerPlaced = true;
    }
}
