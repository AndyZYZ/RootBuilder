using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    [SerializeField]
    Tilemap _tilemap;

    [SerializeField]
    Tile _testTile;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _tilemap.SetTile(_tilemap.WorldToCell(pos), _testTile);
        }
    }
}
