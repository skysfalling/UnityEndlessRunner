using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTileMovement : MonoBehaviour
{
    //global var
    Rigidbody rb;
    public float moveSpeed = 5f;
    public int moveReach = 2;

    public TileManager currTileManager;
    public Vector2 playerCoordinate;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //get the rb
        rb.velocity = Vector3.zero; //init the rb to zero
    }

    void Update()
    {
        MovePlayer();

    }

    public void MovePlayer()
    {
        currTileManager.ChangePointState(playerCoordinate, "PLAYERPOS");
        currTileManager.SetPlayerMovePoints(playerCoordinate);

        Vector2 targetCoord = playerCoordinate;


        //left move input
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //target position to specific spot to the left
            targetCoord = new Vector2(playerCoordinate.x - 1, playerCoordinate.y);

            //print("Left: " + targetCoord);

        }
        //right move
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //target position to specific spot to the right
            targetCoord = new Vector2(playerCoordinate.x + 1, playerCoordinate.y);

            //print("Right: " + targetCoord);

        }

        //forward move input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            //target position to specific spot forward
            targetCoord = new Vector2(playerCoordinate.x, playerCoordinate.y + 1);

            //print("Forward: " + targetCoord);

        }
        //back move
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            //target position to specific spot backward
            targetCoord = new Vector2(playerCoordinate.x, playerCoordinate.y - 1);

            //print("Back: " + targetCoord);

        }

        // ================ MOVEMENT ==================================================================

        //is point in tile a valid point and walkable?
        if (currTileManager.IsPointValid(targetCoord) && currTileManager.IsPointWalkable(targetCoord))
        {
            //if player coordinate not at target coord
            if (playerCoordinate != targetCoord)
            {
                //clear move points
                currTileManager.ClearPlayerMovePoints(playerCoordinate);
            }

            //set target position from target coord
            Vector3 targetPos = currTileManager.GetPoint(targetCoord).worldPos;

            //if player not at position
            if (transform.position != targetPos)
            {
                //move to target position
                transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime); //move to target position
                playerCoordinate = targetCoord; //set player coord to target coord
            }
        }
        //if target in neighboring tile
        else if (currTileManager.IsPointInNeighborTile(targetCoord))
        {
            //get neighboring tile
            TileManager neighborTile = currTileManager.GetNeighborTileFromPoint(targetCoord);
            Vector2 targetCoordInNeighbor = currTileManager.GetPointInNeighborTile(targetCoord).coordinate;

            //check if point is valid and walkable in neighbor tile
            if (neighborTile.IsPointValid(targetCoordInNeighbor) && neighborTile.IsPointWalkable(targetCoordInNeighbor))
            {
                //print("Neighbor Target " + targetCoord);
                print("Move to neighbor: " + targetCoord + " -> " + targetCoordInNeighbor);

                //if player not at target coord
                if (playerCoordinate != targetCoord)
                {
                    currTileManager.ClearPlayerMovePoints(playerCoordinate);
                }

                //get world position of point
                Vector3 targetPos = currTileManager.GetPointInNeighborTile(targetCoord).worldPos;
                //if player not at position
                if (transform.position != targetPos)
                {
                    //move to target position
                    transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime); //move to target position

                    playerCoordinate = currTileManager.GetPointInNeighborTile(targetCoord).coordinate; //change player coordinate to coord from new tile
                    currTileManager = currTileManager.GetNeighborTileFromPoint(targetCoord); //change current player tile from target

                }
            }
        }
    }
}
