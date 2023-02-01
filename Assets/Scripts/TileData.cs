using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileTypes
{
    Root,
    Point,
    Rock,
    Posion,
    Empty
}


[CreateAssetMenu(fileName = "TileData", menuName = "", order = 1)]
public class TileData : ScriptableObject
{
    public string TileName;

    public TileTypes TileType;

    

}
