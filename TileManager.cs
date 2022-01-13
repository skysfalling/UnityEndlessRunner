using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public class TilePoint
    {
        public enum PointState { OPEN, CLOSED, PLAYERPOS, PLAYEROPTION, NULL };

        public PointState pointState;
        public PointState naturalstate;
        public Vector3 worldPos;
        public Vector2 coordinate;

        public TilePoint(Vector3 tile_origin, int x, int y, int tileSize, PointState state = PointState.NULL)
        {
            //                     
            worldPos = new Vector3( (x - (tileSize / 2)) + tile_origin.x, tile_origin.y, (y - (tileSize/2) )+ tile_origin.z);
            coordinate = new Vector2(x, y);
            pointState = state;
            naturalstate = state;
        }

        public void PrintValues()
        {
            print("Coordinates: " + coordinate + " State: " + pointState + " Natural State:" + naturalstate);

        }
    }

    // ===== TILE MANAGER =================================================
    public bool startTile; // is start tile?
    public TileSpawner.Tile thisTile; // the corresponding tile class


    public List<List<TilePoint>> tilePoints = new List<List<TilePoint>>(); //double list for grid of points
    public List<TilePoint> playerMovePoints = new List<TilePoint>(); // list of possible player move points

    public int tileSize; //tile size based on value of tileSize in thisTile

    [Space(10)]
    [Header("Tile Grid Placement")]
    public Vector2 gridPoint; // placement of the tile in the overall world tile grid
    public Vector3 originWorldPos; // tile origin's position in world
    public Vector2 originCoord; // tile origin coord in tilePoints double array
    public List<TileSpawner.Tile> neighborTiles = new List<TileSpawner.Tile>(); // creation of a list of neighbor tiles

    void Start()
    {
        // import tileSize from thisTile
        tileSize = thisTile.tileSize;

        //set origin position based on even or odd tileSize
        if (tileSize % 2 == 0) { originWorldPos = transform.position + new Vector3(0.5f, 1, 0.5f); }
        else { originWorldPos = transform.position + new Vector3(0, 1, 0); }
        
        // set origin coord
        originCoord = new Vector2((int)tileSize / 2, (int)tileSize / 2);

        //CREATE TILEPOINT GRID ==========
        //row
        for (int x = 0; x < tileSize; x++)
        {
            //column
            List<TilePoint> newRow = new List<TilePoint>(); //new row list
            for (int y = 0; y < tileSize; y++)
            {
                TilePoint newPoint = new TilePoint(originWorldPos, x, y, tileSize); //add new point to new row
                newRow.Add(newPoint);
            }

            tilePoints.Add(newRow); //append new row
        }

        // set tile neighbors
        neighborTiles.Add(thisTile.leftNeighbor);
        neighborTiles.Add(thisTile.rightNeighbor);
        neighborTiles.Add(thisTile.topNeighbor);
        neighborTiles.Add(thisTile.bottomNeighbor);

    }

    private void Update()
    {
        // check if neighbor tiles have been updated
        if (neighborTiles[0] == null && thisTile.leftNeighbor != null) { neighborTiles[0] = thisTile.leftNeighbor; }
        if (neighborTiles[1] == null && thisTile.rightNeighbor != null) { neighborTiles[1] = thisTile.rightNeighbor; }
        if (neighborTiles[2] == null && thisTile.topNeighbor != null) { neighborTiles[2] = thisTile.topNeighbor; }
        if (neighborTiles[3] == null && thisTile.bottomNeighbor != null) { neighborTiles[3] = thisTile.bottomNeighbor; }
    }

    //get point in tile
    public TilePoint GetPoint(Vector2 coord)
    {
        return tilePoints[(int)coord.x][(int)coord.y];
    }

    public TilePoint GetPoint(int x, int y)
    {
        return tilePoints[x][y];
    }

    //change state of point in tile
    public void ChangePointState(Vector2 coord, string state_name, bool natural = false)
    {
        TilePoint.PointState state;

        //OPEN, CLOSED, PLAYERPOS, PLAYEROPTION, NULL
        if (state_name == "OPEN") { state = TilePoint.PointState.OPEN; }
        else if (state_name == "CLOSED") { state = TilePoint.PointState.CLOSED; }
        else if (state_name == "PLAYERPOS") { state = TilePoint.PointState.PLAYERPOS; }
        else if (state_name == "PLAYEROPTION") { state = TilePoint.PointState.PLAYEROPTION; }
        else { state = TilePoint.PointState.NULL; }


        if (natural) { GetPoint(coord).naturalstate = state; }
        else { GetPoint(coord).pointState = state; }
    }

    public void ChangePointState(int x, int y, string state_name, bool natural = false)
    {
        TilePoint.PointState state;

        //OPEN, CLOSED, PLAYERPOS, PLAYEROPTION, NULL
        if (state_name == "OPEN") { state = TilePoint.PointState.OPEN; }
        else if (state_name == "CLOSED") { state = TilePoint.PointState.CLOSED; }
        else if (state_name == "PLAYERPOS") { state = TilePoint.PointState.PLAYERPOS; }
        else if (state_name == "PLAYEROPTION") { state = TilePoint.PointState.PLAYEROPTION; }
        else { state = TilePoint.PointState.NULL; }

        if (natural) { GetPoint(x,y).naturalstate = state; }
        else { GetPoint(x,y).pointState = state; }
    }

    //set point range ... rectangle shape
    public void SetPointRange(int x_min, int x_max, int y_min, int y_max, string state)
    {

        for (int x = x_min; x <= x_max; x++)
        {
            for (int y = y_min; y <= y_max; y++)
            {
                ChangePointState(x, y, state);
            }
        }
    }

    //set state of square of points around point
    public void SetSurroundingSquarePoints()
    {

    }

    //set state of points touching point
    public void SetTouchingPoints(Vector2 coord, string state)
    {

    }

    //get possible move points for player
    public void SetPlayerMovePoints(Vector2 playerCoord)
    {
        try
        {
            TilePoint left = GetPoint(new Vector2(playerCoord.x - 1, playerCoord.y));
            // check point left of player
            if (left.pointState != TilePoint.PointState.CLOSED) { ChangePointState(left.coordinate, "PLAYEROPTION"); }
        }
        catch {

            try
            {
                //get point in left neighbor
                TilePoint leftNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x - 1, playerCoord.y));
                //get left neighbor tile
                TileManager leftNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x - 1, playerCoord.y));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (leftNeighborPoint.pointState != TilePoint.PointState.CLOSED) { leftNeighbor.ChangePointState(leftNeighborPoint.coordinate, "PLAYEROPTION"); }
            }
            catch { print("Left Point from player does not exist"); }

        }

        try
        {
            TilePoint right = GetPoint(new Vector2(playerCoord.x + 1, playerCoord.y));
            // check point right of player
            if (right.pointState != TilePoint.PointState.CLOSED) { ChangePointState(right.coordinate, "PLAYEROPTION"); }
        }
        catch
        {
            try
            {
                //get point in right neighbor
                TilePoint rightNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x + 1, playerCoord.y));
                //get right neighbor tile
                TileManager rightNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x + 1, playerCoord.y));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (rightNeighborPoint.pointState != TilePoint.PointState.CLOSED) { rightNeighbor.ChangePointState(rightNeighborPoint.coordinate, "PLAYEROPTION"); }
            }
            catch { print("Right Point from player does not exist"); }
        }

        try
        {
            TilePoint forward = GetPoint(new Vector2(playerCoord.x, playerCoord.y + 1));
            // check point forward of player
            if (forward.pointState != TilePoint.PointState.CLOSED) { ChangePointState(forward.coordinate, "PLAYEROPTION"); }
        }
        catch
        {
            try
            {
                //get point in forward neighbor
                TilePoint forwardNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x, playerCoord.y + 1));
                //get forward neighbor tile
                TileManager forwardNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x, playerCoord.y + 1));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (forwardNeighborPoint.pointState != TilePoint.PointState.CLOSED) { forwardNeighbor.ChangePointState(forwardNeighborPoint.coordinate, "PLAYEROPTION"); }
            }
            catch { print("Forward Point from player does not exist"); }
        }


        try
        {
            TilePoint back = GetPoint(new Vector2(playerCoord.x, playerCoord.y - 1));
            // check point back of player
            if (back.pointState != TilePoint.PointState.CLOSED) { ChangePointState(back.coordinate, "PLAYEROPTION"); }
        }
        catch
        {
            try
            {
                //get point in back neighbor
                TilePoint backNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x, playerCoord.y - 1));
                //get back neighbor tile
                TileManager backNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x, playerCoord.y - 1));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (backNeighborPoint.pointState != TilePoint.PointState.CLOSED) { backNeighbor.ChangePointState(backNeighborPoint.coordinate, "PLAYEROPTION"); }
            }
            catch { print("Back Point from player does not exist"); }
        }
    }

    //clear move points
    public void ClearPlayerMovePoints(Vector2 playerCoord)
    {

        try
        {
            TilePoint left = GetPoint(new Vector2(playerCoord.x - 1, playerCoord.y));
            // check point left of player
            if (left.pointState == TilePoint.PointState.PLAYEROPTION) { ChangePointState(left.coordinate, left.naturalstate.ToString()); }
        }
        catch
        {

            try
            {
                //get point in left neighbor
                TilePoint leftNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x - 1, playerCoord.y));
                //get left neighbor tile
                TileManager leftNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x - 1, playerCoord.y));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (leftNeighborPoint.pointState == TilePoint.PointState.PLAYEROPTION) { leftNeighbor.ChangePointState(leftNeighborPoint.coordinate, leftNeighborPoint.naturalstate.ToString()); }
            }
            catch { print("Left Point from player does not exist"); }

        }

        try
        {
            TilePoint right = GetPoint(new Vector2(playerCoord.x + 1, playerCoord.y));
            // check point right of player
            if (right.pointState == TilePoint.PointState.PLAYEROPTION) { ChangePointState(right.coordinate, right.naturalstate.ToString()); }
        }
        catch
        {

            try
            {
                //get point in left neighbor
                TilePoint rightNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x + 1, playerCoord.y));
                //get left neighbor tile
                TileManager rightNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x + 1, playerCoord.y));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (rightNeighborPoint.pointState == TilePoint.PointState.PLAYEROPTION) { rightNeighbor.ChangePointState(rightNeighborPoint.coordinate, rightNeighborPoint.naturalstate.ToString()); }
            }
            catch { print("Right Point from player does not exist"); }

        }

        try
        {
            TilePoint forward = GetPoint(new Vector2(playerCoord.x, playerCoord.y + 1));
            // check point right of player
            if (forward.pointState == TilePoint.PointState.PLAYEROPTION) { ChangePointState(forward.coordinate, forward.naturalstate.ToString()); }
        }
        catch
        {

            try
            {
                //get point in left neighbor
                TilePoint forwardNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x, playerCoord.y + 1));
                //get left neighbor tile
                TileManager forwardNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x, playerCoord.y + 1));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (forwardNeighborPoint.pointState == TilePoint.PointState.PLAYEROPTION) { forwardNeighbor.ChangePointState(forwardNeighborPoint.coordinate, forwardNeighborPoint.naturalstate.ToString()); }
            }
            catch { print("Forward Point from player does not exist"); }

        }

        try
        {
            TilePoint back = GetPoint(new Vector2(playerCoord.x, playerCoord.y - 1));
            // check point right of player
            if (back.pointState == TilePoint.PointState.PLAYEROPTION) { ChangePointState(back.coordinate, back.naturalstate.ToString()); }
        }
        catch
        {

            try
            {
                //get point in left neighbor
                TilePoint backNeighborPoint = GetPointInNeighborTile(new Vector2(playerCoord.x, playerCoord.y - 1));
                //get left neighbor tile
                TileManager backNeighbor = GetNeighborTileFromPoint(new Vector2(playerCoord.x, playerCoord.y - 1));
                //if point != CLOSED, set point to "PLAYEROPTION"
                if (backNeighborPoint.pointState == TilePoint.PointState.PLAYEROPTION) { backNeighbor.ChangePointState(backNeighborPoint.coordinate, backNeighborPoint.naturalstate.ToString()); }
            }
            catch { print("Forward Point from player does not exist"); }

        }
    }

    //check if point is valid
    public bool IsPointValid(Vector2 coord)
    {
        foreach (List<TilePoint> pointList in tilePoints)
        {
            foreach (TilePoint point in pointList)
            { 
                if (point.coordinate == coord)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //check if point is player moveable
    public bool IsPointWalkable(Vector2 coord)
    {
        TilePoint point = GetPoint(coord);
        if (point.pointState == TilePoint.PointState.CLOSED)
        {
            return false;
        }

        return true;
    }

    // == NEIGHBOR TILES ===================
    //check if point in neighbor tiles
    public bool IsPointInNeighborTile(Vector2 coord)
    {
        print("is point in neighbor" + coord);

        //check left neighbor
        if (coord.x < 0 && thisTile.leftNeighbor != null) 
        {
            print ("Check left neighbor: " + coord + " -> " + new Vector2((int)coord.x + tileSize, (int)coord.y));
            thisTile.leftNeighbor.tileManager.GetPoint((int)coord.x + 30, (int)coord.y);
            return true;
        }

        //check right neighbor
        else if (coord.x > tileSize - 1 && thisTile.rightNeighbor != null)
        {
            print("Check right neighbor: " + coord + " -> " + new Vector2((int)coord.x - tileSize, (int)coord.y));
            thisTile.rightNeighbor.tileManager.GetPoint((int)coord.x - 30, (int)coord.y);
            return true;
        }

        //check top neighbor
        else if (coord.y < 0 && thisTile.topNeighbor != null)
        {
            thisTile.topNeighbor.tileManager.GetPoint((int)coord.x, (int)coord.y + tileSize);
            return true;
        }

        //check bottom neighbor
        else if (coord.y > tileSize - 1 && thisTile.bottomNeighbor != null)
        {
            thisTile.bottomNeighbor.tileManager.GetPoint((int)coord.x, (int)coord.y - tileSize);
            return true;
        }

        else { return false; }
    }

    public TileManager GetNeighborTileFromPoint(Vector2 coord)
    {
        //check left neighbor
        if (coord.x < 0 && thisTile.leftNeighbor != null)
        {
            return thisTile.leftNeighbor.tileManager;
        }

        //check right neighbor
        else if (coord.x > tileSize - 1 && thisTile.rightNeighbor != null)
        {
            return thisTile.rightNeighbor.tileManager;
        }

        //check top neighbor
        else if (coord.y < 0 && thisTile.topNeighbor != null)
        {
            return thisTile.topNeighbor.tileManager;
        }

        //check bottom neighbor
        else if (coord.y > tileSize - 1 && thisTile.bottomNeighbor != null)
        {
            return thisTile.bottomNeighbor.tileManager;
        }

        else { return null; }
    }

    public TilePoint GetPointInNeighborTile(Vector2 coord)
    {
        TilePoint point = null;

        //check left neighbor
        if (coord.x < 0 && thisTile.leftNeighbor != null)
        {
            point = thisTile.leftNeighbor.tileManager.GetPoint((int)coord.x + tileSize, (int)coord.y);
        }

        //check right neighbor
        else if (coord.x > tileSize - 1 && thisTile.rightNeighbor != null)
        {
            point = thisTile.rightNeighbor.tileManager.GetPoint((int)coord.x - tileSize, (int)coord.y);
        }

        //check top neighbor
        else if (coord.y < 0 && thisTile.topNeighbor != null)
        {
            point = thisTile.topNeighbor.tileManager.GetPoint((int)coord.x, (int)coord.y + tileSize);
        }

        //check bottom neighbor
        else if (coord.y > tileSize - 1 && thisTile.bottomNeighbor != null)
        {
            point = thisTile.bottomNeighbor.tileManager.GetPoint((int)coord.x, (int)coord.y - tileSize);
        }

        return point;
    }
    

    public void OnDrawGizmos()
    {
        foreach (List<TilePoint> pointList in tilePoints)
        {
            foreach (TilePoint point in pointList)
            {
                // ===== CHANGE COLOR BASED ON STATE ==================================================
                //if state NULL
                if (point.pointState == TilePoint.PointState.NULL) { Gizmos.color = Color.white; }
                //if state OPEN (for player move)
                if (point.pointState == TilePoint.PointState.OPEN) { Gizmos.color = Color.green; }
                //if state CLOSED (to player move)
                if (point.pointState == TilePoint.PointState.CLOSED) { Gizmos.color = Color.black; }
                //if state PLAYERPOS (cur player pos)
                if (point.pointState == TilePoint.PointState.PLAYERPOS) { Gizmos.color = Color.red; }
                //if state PLAYEROPTION (cur player pos)
                if (point.pointState == TilePoint.PointState.PLAYEROPTION) { Gizmos.color = Color.magenta; }

                //Draw Cube
                Gizmos.DrawCube(point.worldPos, Vector3.one * 0.25f);

                //Draw Line
                Gizmos.color = Color.white;
                Gizmos.DrawLine(point.worldPos + Vector3.down, point.worldPos);
            }
        }
    }
}
