using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class BuildManager : MonoBehaviourSingleton<BuildManager>
{
   

    public TileDataDic DataDic;
    public UnityAction OnPointGet;

    public struct RootBuilderTile
    {
        public RootBuilderTile(TileBase rbtile, List<TileData> neibours)
        {
            RBTile = rbtile;
            Neibours = neibours;
        }
   
        public TileBase RBTile { get; private set; }
        public List<TileData> Neibours { get; private set; }

    }

    [SerializeField]
    Tilemap _tilemap;

    [SerializeField]
    TileBase _testTile;

    [SerializeField]
    TileBase[] _randomPool;

    [SerializeField]
    TileData _outOfBoundTileData;

    [SerializeField]
    TileBase[] _dirtTilePool;

    List<Vector3> _buildPath;

    [SerializeField]
    AudioClip _placeAudio;

    [SerializeField]
    AudioClip _removeAudio;

    [SerializeField]
    GameObject _viewPoint;

    AudioSource _buildAudio;

    private void Awake()
    {
        GameManager.Instance.OnInitialzed += ClearBuildPath;
        _buildAudio = this.GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnInitialzed -= ClearBuildPath;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlaceTile();
        if (Input.GetMouseButtonDown(1))
            RemoveRoot();
    }

    void PlaceTile()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rbTile = GetRBTileOnClick(pos);

        
        if (CanPlaceRoot(rbTile) && HasRootLeft())
        {
            _tilemap.SetTile(_tilemap.WorldToCell(pos), _testTile);
            GameManager.Instance.Remove1Root();
            if (GetPoint(rbTile) > 0)
                GameManager.Instance.AddScore(GetPoint(rbTile));
            _buildPath.Add(pos);

            _buildAudio.PlayOneShot(_placeAudio);

            _viewPoint.transform.position = pos;
        }
    }
    void RemoveRoot()
    {
        //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //var rbTile = GetRBTileOnClick(pos);

        //if(IsRoot(rbTile) && IsConnected(rbTile))
        //{
        //    _tilemap.SetTile(_tilemap.WorldToCell(pos), GetRandomDirtTile());
        //    GameManager.Instance.Add1Root();
        //    if (GetPoint(rbTile) > 0)
        //        GameManager.Instance.AddScore(-GetPoint(rbTile));
        //}
        if (_buildPath.Count == 0)
            return;
        
        var pos = _buildPath[_buildPath.Count - 1];
        var rbTile = GetRBTileOnClick(pos);
        
        if (GetPoint(rbTile) > 0)
            GameManager.Instance.AddScore(-GetPoint(rbTile));

        _tilemap.SetTile(_tilemap.WorldToCell(pos), GetRandomDirtTile());
        GameManager.Instance.Add1Root();

        _viewPoint.transform.position = pos;

        _buildPath.RemoveAt(_buildPath.Count - 1);

        _buildAudio.PlayOneShot(_removeAudio);

    }


    bool HasRootLeft()
    {
        if (GameManager.Instance.RootRemain > 0)
            return true;
        else
            return false;
    }

    int GetPoint(RootBuilderTile rbTile)
    {
        var point = 0;
        foreach (var neibour in rbTile.Neibours)
        {
            if (neibour.TileType == TileTypes.Point)
                point++;
        }

        return point;
    }

    bool CanPlaceRoot(RootBuilderTile target)
    {
        
        if(IsEmpty(target) && RootNearby(target))
            return true;
        else
            return false;
    }

    bool IsEmpty(RootBuilderTile target)
    {
        if (GetTileData(target.RBTile).TileType == TileTypes.Empty)
            return true;
        else 
            return false;
    }

    bool RootNearby(RootBuilderTile target)
    {
        for (var i = 0; i < target.Neibours.Count-1; i++)
            if (target.Neibours[i].TileType == TileTypes.Root)
                return true;
        return false;
    }

    bool IsRoot(RootBuilderTile target)
    {
        return GetTileData(target.RBTile).TileType == TileTypes.Root ? true : false;
    }

    bool IsConnected(RootBuilderTile target)
    {
        for (int i = 0; i < target.Neibours.Count - 1; i++)
            if (target.Neibours[i].TileType == TileTypes.Root)
                return true;

        return false;
    }

    TileBase GetRandomDirtTile()
    {
        int rng = Random.Range(0, 4);
        return _dirtTilePool[rng];
            
    }
    TileData GetTileData(TileBase target)
    {
        return DataDic.DataDic[target];
    }

    RootBuilderTile GetRBTileOnClick(Vector3 pos)
    {
        var tpos = _tilemap.WorldToCell(pos);

        var tile = _tilemap.GetTile(tpos);
        List<TileData> neibours = new List<TileData>();
        var tmpTile = _tilemap.GetTile(tpos + Vector3Int.left);
        if (tmpTile != null)
            neibours.Add(GetTileData(tmpTile));
        else   
            neibours.Add(_outOfBoundTileData);
        tmpTile = _tilemap.GetTile(tpos + Vector3Int.up);
        if (tmpTile != null)
            neibours.Add(GetTileData(tmpTile));
        else
            neibours.Add(_outOfBoundTileData);
        tmpTile = _tilemap.GetTile(tpos + Vector3Int.right);
        if (tmpTile != null)
            neibours.Add(GetTileData(tmpTile));
        else
            neibours.Add(_outOfBoundTileData);
        tmpTile = _tilemap.GetTile(tpos + Vector3Int.down);
        if (tmpTile != null)
            neibours.Add(GetTileData(tmpTile));
        else
            neibours.Add(_outOfBoundTileData);
        var rbTile = new RootBuilderTile(tile, neibours);
        return rbTile;
    }

    [Button("Generate Random Tilemap")]
    public void GenerateTileMap(int height, int width)
    {
        for(int i = -width; i <= width; i++)
        {
            for(int j = 0; j >= -height; j--)
            {
                var pos = new Vector3(i, j, 0);
                _tilemap.SetTile(_tilemap.WorldToCell(pos), GetRandomTile());
            }
        }

        var start = new Vector3(0, 0, 0);
        _tilemap.SetTile(_tilemap.WorldToCell(start), _testTile);
    }

    TileBase GetRandomTile()
    {
        int rng = Random.Range(0, 10);

        return _randomPool[rng];
    }

    void ClearBuildPath()
    {
        _buildPath = new List<Vector3>();
    }

}



