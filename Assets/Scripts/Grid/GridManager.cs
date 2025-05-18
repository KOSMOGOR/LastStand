using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public Tile tilePrefab;
    public int gridWidth = 3, gridHeight = 5;
    public float tileXOffset = 1, tileYOffset = 1;
    public GameObject tileSelectionSinglePrefab;
    public GameObject tileSelectionMultiplePrefab;
    public GameObject hoverSelectionSinglePrefab;
    public GameObject hoverSelectionMultiplePrefab;
    public Transform playerTransform;
    public float playerPositionMultiplier = 1;

    [Header("Internal")]
    public Tile currentSelectedTile;
    public List<Tile> additionalSelectedTiles = new();
    public GridSelectionType currentGridSelectionType = GridSelectionType.None;
    GameObject tileSelectionSingle;
    List<GameObject> tileSelectionMultiple;
    GameObject hoverSelectionSingle;
    List<GameObject> hoverSelectionMultiple;
    public Dictionary<Vector2Int, Tile> tiles = new();

    public static GridManager I;

    void Awake() {
        if (I != null) Destroy(gameObject);
        I = this;
        GameObject tileSelectionParent = new("TileSelection");
        tileSelectionSingle = Instantiate(tileSelectionSinglePrefab, tileSelectionParent.transform);
        tileSelectionMultiple = Enumerable.Repeat<GameObject>(null, gridWidth + gridHeight).Select(_ => Instantiate(tileSelectionMultiplePrefab, tileSelectionParent.transform)).ToList();
        HideTileSelectionObjects();
        hoverSelectionSingle = Instantiate(hoverSelectionSinglePrefab, tileSelectionParent.transform);
        hoverSelectionMultiple = Enumerable.Repeat<GameObject>(null, gridWidth + gridHeight).Select(_ => Instantiate(hoverSelectionMultiplePrefab, tileSelectionParent.transform)).ToList();
        HideHoverSelectionObjects();
    }

    public void MaybeGenerateGrid() {
        if (tiles.Count != gridWidth * gridWidth) GenerateGrid();
    }

    [ContextMenu("RemoveGrid")]
    public void RemoveGrid() {
        foreach (var i in GetComponentsInChildren<Tile>()) {
            DestroyImmediate(i.gameObject);
        }
        tiles = new();
    }

    [ContextMenu("GenerateGrid")]
    public void GenerateGrid() {
        RemoveGrid();
        float gridXMid = (gridWidth - 1) / 2, gridYMid = (gridHeight - 1) / 2;
        for (int x = 0; x < gridWidth; ++x) {
            for (int y = gridHeight - 1; y >= 0; --y) {
                Vector2 tilePos = new((x - gridXMid) * tileXOffset, (y - gridYMid) * tileYOffset);
                Tile tile = Instantiate(tilePrefab, (Vector3)tilePos + new Vector3(0, 0, (float)y / 10) + transform.position, Quaternion.identity, transform);
                tile.name = $"{tilePrefab.name} {x} {y}";
                tile.xy = new Vector2Int(x, y);
                tiles[new(x, y)] = tile;
            }
        }
    }

    Tile GetTileInternal(Vector2Int coords) {
        return tiles[coords];
    }

    public Tile GetTile(Vector2Int coords) {
        if (coords.x < 0 || coords.x >= gridWidth || coords.y < 0 || coords.y >= gridHeight) return null;
        return GetTileInternal(coords);
    }

    public Tile GetTile(int x, int y) {
        return GetTile(new Vector2Int(x, y));
    }

    public Tile GetTileSafe(Vector2Int coords) {
        coords.x = Math.Clamp(coords.x, 0, gridWidth);
        coords.y = Math.Clamp(coords.y, 0, gridHeight);
        return GetTileInternal(coords);
    }

    public List<BaseZombie> GetAllZombies() {
        List<BaseZombie> zombies = new();
        foreach (Tile tile in tiles.Values) zombies.AddRange(tile.zombies);
        return zombies;
    }

    public int GetZombiesCount() {
        int count = 0;
        foreach (Tile tile in tiles.Values) count += tile.zombies.Count;
        return count;
    }

    public Tile GetTileToSpawnZombie() {
        Vector2Int pos = new(Random.Range(0, gridWidth), gridHeight - 1);
        return tiles[pos];
    }

    public void ChooseTile(Tile tile) {
        HideTileSelectionObjects();
        if (currentGridSelectionType == GridSelectionType.None) return;
        MovePlayerXToTile(tile);
        EmptySelection();
        currentSelectedTile = tile;
        tile.SetSelected(true);
        switch (currentGridSelectionType) {
            case GridSelectionType.Tile:
                SetSingleSelectionTile(tile);
                break;
            case GridSelectionType.Column:
                for (int y = 0; y < gridHeight; ++y) {
                    Tile additionalTile = GetTile(tile.xy.x, y);
                    additionalTile.SetSelected(true);
                    SetMultipleSelectionTile(additionalTile);
                    if (y != tile.xy.y) additionalSelectedTiles.Add(additionalTile);
                }
                break; 
            default: break;
        }
    }

    public void HoverTile(Tile tile) {
        HideHoverSelectionObjects();
        if (tile == null) return;
        if (currentGridSelectionType == GridSelectionType.None) return;
        if (currentSelectedTile == null) MovePlayerXToTile(tile);
        switch (currentGridSelectionType) {
            case GridSelectionType.Tile:
                SetSingleSelectionHover(tile);
                break;
            case GridSelectionType.Column:
                for (int y = 0; y < gridHeight; ++y) {
                    Tile additionalTile = GetTile(tile.xy.x, y);
                    SetMultipleSelectionHover(additionalTile);
                }
                break; 
            default: break;
        }
    }

    public void SetCurrentGridSelectionType(GridSelectionType gridSelectionType) {
        if (gridSelectionType == GridSelectionType.None) EmptySelection();
        currentGridSelectionType = gridSelectionType;
    }

    void EmptySelection() {
        if (currentSelectedTile) {
            currentSelectedTile.SetSelected(false);
            currentSelectedTile = null;
        }
        foreach (Tile tile in additionalSelectedTiles) tile.SetSelected(false);
        additionalSelectedTiles.Clear();
        HideTileSelectionObjects();
    }

    void SetSingleSelectionTile(Tile tile) {
        tileSelectionSingle.SetActive(true);
        tileSelectionSingle.transform.SetPositionAndRotation(tile.transform.position, tile.transform.rotation);
    }

    void SetMultipleSelectionTile(Tile tile) {
        GameObject tileSelection = tileSelectionMultiple.Find(ts => !ts.activeInHierarchy);
        tileSelection.SetActive(true);
        tileSelection.transform.SetPositionAndRotation(tile.transform.position, tile.transform.rotation);
    }

    void SetSingleSelectionHover(Tile tile) {
        hoverSelectionSingle.SetActive(true);
        hoverSelectionSingle.transform.SetPositionAndRotation(tile.transform.position, tile.transform.rotation);
    }

    void SetMultipleSelectionHover(Tile tile) {
        GameObject hoverSelection = hoverSelectionMultiple.Find(ts => !ts.activeInHierarchy);
        hoverSelection.SetActive(true);
        hoverSelection.transform.SetPositionAndRotation(tile.transform.position, tile.transform.rotation);
    }

    void HideTileSelectionObjects() {
        tileSelectionSingle.SetActive(false);
        tileSelectionMultiple.ForEach(ts => ts.SetActive(false));
    }

    void HideHoverSelectionObjects() {
        hoverSelectionSingle.SetActive(false);
        hoverSelectionMultiple.ForEach(ts => ts.SetActive(false));
    }

    public List<Tile> GetAllTilesInColumn(int x) {
        List<Tile> columnTiles = new();
        for (int y = 0; y < gridHeight; ++y) columnTiles.Add(GetTile(x, y));
        return columnTiles;
    }

    void MovePlayerXToTile(Tile tile) {
        Vector3 pos = playerTransform.position;
        pos.x = (tile.transform.position.x - transform.position.x) * playerPositionMultiplier + transform.position.x;
        playerTransform.position = pos;
    }

    public void SetZombieVisibility() {
        foreach (Tile tile in tiles.Values) {
            for (int i = 0; i < tile.zombies.Count; ++i)
                tile.zombies[i].SetVisible(i == 0);
        }
    }
}

public static class MovementDirection {
    public static Vector2Int DOWN = new(0, -1);
    public static Vector2Int UP = new(0, 1);
    public static Vector2Int LEFT = new(-1, 0);
    public static Vector2Int RIGHT = new(1, 0);
}

public enum GridSelectionType {
    None,
    Tile,
    Column
}
