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
        public RootBuilderTile(TileBase rbtile, List<TileData> neibours, int[] pointFlag)
        {
            RBTile = rbtile;
            Neibours = neibours;
            PointFlag = pointFlag;
        }
   
        public TileBase RBTile { get; private set; }
        public List<TileData> Neibours { get; private set; }

        public int[] PointFlag;

    }

    [SerializeField]
    Tilemap _tilemap;

    [SerializeField]
    Tilemap _rootTileMap;

    [SerializeField]
    TileBase _testTile;

    [SerializeField]
    TileBase _ruleTile;

    [SerializeField]
    TileBase _heartTile;

    [SerializeField]
    TileBase[] _randomPool;

    [SerializeField]
    TileData _outOfBoundTileData;

    [SerializeField]
    TileBase[] _dirtTilePool;

    [SerializeField]
    TileBase _lightedTile;

    [SerializeField]
    TileBase _darkedTile;

    List<Vector3> _buildPath;

    [SerializeField]
    AudioClip _placeAudio;

    [SerializeField]
    AudioClip _removeAudio;

    [SerializeField]
    GameObject _viewPointX;
    [SerializeField]
    GameObject _viewPointY;

    AudioSource _buildAudio;

    private void Awake()
    {
        GameManager.Instance.OnInitialzed += ClearBuildPath;
        GameManager.Instance.OnInitialzed += ClearRootTile;
        GameManager.Instance.OnInitialzed += ResetViewpoint;
        _buildAudio = this.GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnInitialzed -= ClearBuildPath;
        GameManager.Instance.OnInitialzed -= ClearRootTile;
        GameManager.Instance.OnInitialzed -= ResetViewpoint;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = _tilemap.GetTile(_tilemap.WorldToCell(pos));
            if (tile)
            {
                PlaceTile(pos);
            }
        }
            
        if (Input.GetMouseButtonDown(1))
            RemoveRoot();
    }

    void PlaceTile(Vector3 pos)
    {
        var rbTile = GetRBTileOnClick(pos);


        if (CanPlaceRoot(rbTile) && HasRootLeft())
        {
            _tilemap.SetTile(_tilemap.WorldToCell(pos), _testTile);
            _rootTileMap.SetTile(_rootTileMap.WorldToCell(pos), _ruleTile);
            GameManager.Instance.Remove1Root();
            if (GetPoint(rbTile) > 0)
            {
                var rbt = GetPoint(rbTile);
                GameManager.Instance.AddScore(rbt);

            }

            _buildPath.Add(pos);

            _buildAudio.PlayOneShot(_placeAudio);

            if (pos.y < _viewPointY.transform.position.y)
                _viewPointY.transform.position = pos;

            if (Mathf.Abs(pos.x) > Mathf.Abs(_viewPointX.transform.position.x))
                _viewPointX.transform.position = pos;
                

            LightPoint(rbTile.PointFlag,pos);
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
        _rootTileMap.SetTile(_tilemap.WorldToCell(pos), null);
        GameManager.Instance.Add1Root();

        DarkPoint(rbTile.PointFlag, pos);

        _buildPath.RemoveAt(_buildPath.Count - 1);

        _buildAudio.PlayOneShot(_removeAudio);

    }


    void LightPoint(int[] dir, Vector3 pos)
    {
        if(dir[0] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos+Vector3Int.left), _lightedTile);
        if (dir[1] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos + Vector3Int.up), _lightedTile);
        if (dir[2] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos + Vector3Int.right), _lightedTile);
        if (dir[3] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos + Vector3Int.down), _lightedTile);
    }

    void DarkPoint(int[] dir, Vector3 pos)
    {
        if (dir[0] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos + Vector3Int.left), _darkedTile);
        if (dir[1] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos + Vector3Int.up), _darkedTile);
        if (dir[2] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos + Vector3Int.right), _darkedTile);
        if (dir[3] == 1)
            _tilemap.SetTile(_tilemap.WorldToCell(pos + Vector3Int.down), _darkedTile);
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
        int[] pointFlag = { 0, 0, 0, 0 };
        var tmpTile = _tilemap.GetTile(tpos + Vector3Int.left);
        if (tmpTile != null)
        {
            var tmprbt = GetTileData(tmpTile);
            neibours.Add(tmprbt);
            if (tmprbt.TileType == TileTypes.Point)
            {
                pointFlag[0] = 1;
            }
        }
        else   
            neibours.Add(_outOfBoundTileData);

        tmpTile = _tilemap.GetTile(tpos + Vector3Int.up);
        if (tmpTile != null)
        {
            var tmprbt = GetTileData(tmpTile);
            neibours.Add(tmprbt);
            if (tmprbt.TileType == TileTypes.Point)
            {
                pointFlag[1] = 1;
            }
        }
        else
            neibours.Add(_outOfBoundTileData);

        tmpTile = _tilemap.GetTile(tpos + Vector3Int.right);
        if (tmpTile != null)
        {
            var tmprbt = GetTileData(tmpTile);
            neibours.Add(tmprbt);
            if (tmprbt.TileType == TileTypes.Point)
            {
                pointFlag[2] = 1;
            }
        }
        else
            neibours.Add(_outOfBoundTileData);

        tmpTile = _tilemap.GetTile(tpos + Vector3Int.down);
        if (tmpTile != null)
        {
            var tmprbt = GetTileData(tmpTile);
            neibours.Add(tmprbt);
            if (tmprbt.TileType == TileTypes.Point)
            {
                pointFlag[3] = 1;
            }
        }
        else
            neibours.Add(_outOfBoundTileData);

        var rbTile = new RootBuilderTile(tile, neibours,pointFlag);
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
        _tilemap.SetTile(_tilemap.WorldToCell(start+Vector3Int.left), GetRandomDirtTile());
        _tilemap.SetTile(_tilemap.WorldToCell(start + Vector3Int.right), GetRandomDirtTile());
        _tilemap.SetTile(_tilemap.WorldToCell(start + Vector3Int.down), GetRandomDirtTile());
        _rootTileMap.SetTile(_tilemap.WorldToCell(start), _ruleTile);
    }

    TileBase GetRandomTile()
    {
        int rng = Random.Range(0,_randomPool.Length);

        return _randomPool[rng];
    }

    void ClearBuildPath()
    {
        _buildPath = new List<Vector3>();
    }

    void ClearRootTile()
    {
        _rootTileMap.ClearAllTiles();
    }

    void ResetViewpoint()
    {
        _viewPointX.transform.position = new Vector3(0, 0, 0);
        _viewPointY.transform.position = new Vector3(0, 0, 0);
    }

}



