using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "TileDataDic", menuName = "", order = 1)]
public class TileDataDic : SerializedScriptableObject
{
    public Dictionary<TileBase, TileData> DataDic = new Dictionary<TileBase, TileData>();
}
