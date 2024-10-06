using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    // tiles and rows are children
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }

    // get the array's length  and  rows' size 
    public int size => cells.Length;  
    public int height => rows.Length;

    public int width => size / height;
    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)   // row
        {
            for (int x = 0; x < rows[y].cells.Length; x++)  // col
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    // return legal cell
    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {

            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }

    // give the coordinate return the cell
    // it is not necessary just to make sure we can use more cleaner
    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }
    // to move , we need know the adjcent cell to make sure 
    // we can move the tiles !
    public TileCell GetAdjacentCell(TileCell cell ,Vector2Int direction )
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }


    // get a random cell from the grid
    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;
        while (cells[index].occpuied )
        {
            index++;
            // when it is last cell in the grid 
            // go back to the start
            if(index >= cells.Length)
            {
                index = 0;
            }

            // to avoid all the cells are occpuied
            // add a loop to make sure it would not 
            // equal the startingIndex 
            if(startingIndex == index)
            {
                return null;
            }
        }

        return cells[index];
    }
}
