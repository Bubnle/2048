using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;

    private TileGrid grid;

    private List<Tile> tiles;

    public Tile tilePrefab;

    public TileState[] tileStates;

    // wait for  the state steady
    private bool waiting;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }
    // every time game starts create two tiles !
   
    public void ClearBoard()
    {
        foreach(var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
               Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab,grid.transform);
        tile.SetState(tileStates[0], 2);
        // create a tile according to the cell pass in
        tile.Spawn(grid.GetRandomEmptyCell());

        tiles.Add(tile);
    }

    // when get the singal from the keyboard 
    // move the tiles !
   private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }

    }
    // it is a little complicated
    // we use startX , startY control the start pos
    // use incrementX , incrementY to control the direction
    // we want to search like 1 or -1
    private void MoveTiles(Vector2Int direction ,int startX , int incrementX ,int startY , int incrementY)
    {
        // to make sure the change is finished
        bool changed = false;
        for (int x = startX;x >= 0  && x < grid.width; x += incrementX)
        {
            for(int y = startY; y >= 0 && y< grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x,y);

                if (cell.occpuied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }
        // if it already have been changed strat the coroutine
        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }


    }
    // main logical fun of the game 
    // move just one tile
    private bool MoveTile(Tile tile , Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell,direction);

        while (adjacent != null) 
        {
            if (adjacent.occpuied)
            {
                // merge !
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            // when we move 
            // adjacent one should be updated
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a ,Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a ,Tile b)
    {
        tiles.Remove(a);

        // let a merge to the cell which has b
        a.Merge(b.cell);

        // limited in the array'length
        int index = Mathf.Clamp(IndexOf(b.state)+1,0,tileStates.Length-1);

        Debug.Log($"Before SetState: Current State: {b.state}, Number: {b.number}, Calculated Index: {index}");


        int number = b.number * 2;

        TileState newState = tileStates[index];
        //Debug.Log($"TileState index: {index}, Background Color: {tileStates[index].backgroundColor}, Text Color: {tileStates[index].textColor}");

        b.SetState(newState,number);

        gameManager.IncreaseScore(number);
        
    }

    private int IndexOf(TileState state)
    {
        for(int i = 0; i < tileStates.Length; i++)
        {
            if(state == tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        // after 0.1 second we can move again 
        waiting = false;


        foreach(var tile in tiles) { 
            tile.locked = false;
        }
        if(tiles.Count != grid.size)
        {
            CreateTile();
        }

        if (CheckGameOver())
        {
            gameManager.GameOver();
        }
        
    }
    // to check if the game can continue
    private bool CheckGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }
        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if(up!=null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }

        }
        return true;
    }
}
