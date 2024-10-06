using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    // a vector contains x and y 
    public Vector2Int coordinates { get; set; }
    // every cell can set and get 
    public Tile tile { get; set; }
    // state to judge cell
    public bool empty => tile == null;
    public bool occpuied => tile != null;
}
