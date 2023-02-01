using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviourSingleton<BuildManager>
{
    [SerializeField]
    Tilemap _tilemap;

    [SerializeField]
    TileBase _testTile;

    public TileDataDic DataDic;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTile();
        }
    }

    void PlaceTile()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var tpos = _tilemap.WorldToCell(pos);
        var tile = _tilemap.GetTile(tpos);
        if (CanPlaceRoot(tile))
            _tilemap.SetTile(_tilemap.WorldToCell(pos), _testTile);
    }

    bool CanPlaceRoot(TileBase target)
    {
        if(GetTileData(target).TileType == TileTypes.Empty)
            return true;
        else
            return false;
    }

    TileData GetTileData(TileBase target)
    {
        return DataDic.DataDic[target];
    }

}
