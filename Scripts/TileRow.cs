using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    //  it cotains tilecells  all become an array
    public TileCell[] cells {  get; private set; }

    private void Awake()
    {
        // get children components!!  
        cells = GetComponentsInChildren<TileCell>();

    }
}
