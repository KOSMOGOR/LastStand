using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Sprite> sprites;
    public Vector2Int xy;
    public TMP_Text zombieCountText;
    public Vector3 zombieOffset;

    public List<BaseZombie> zombies = new();
    public List<BaseObstacle> obstacles = new();
    public bool isSelected = false;

    void Start() {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count)];
    }

    void Update() {
        zombieCountText.text = zombies.Count > 1 ? $"x{zombies.Count}" : "";
    }

    void OnMouseDown() {
        GridManager.I.ChooseTile(this);
    }

    void OnMouseEnter() {
        GridManager.I.HoverTile(this);
    }

    void OnMouseExit() {
        GridManager.I.HoverTile(null);
    }

    public void SetSelected(bool selected) {
        isSelected = selected;
    }

    public BaseZombie GetFirstZombie() {
        if (zombies.Count > 0) return zombies.First();
        return null;
    }

    public BaseObstacle GetFirstBlockingObstacle() {
        List<BaseObstacle> blockingObstacles = obstacles.Where(o => o.blocking).ToList();
        if (blockingObstacles.Count > 0) return blockingObstacles.First();
        return null;
    }

    public BaseObstacle GetFirstBlockingProjectilesObstacle() {
        List<BaseObstacle> blockingProjectilesObstacles = obstacles.Where(o => o.blockingProjectiles).ToList();
        if (blockingProjectilesObstacles.Count > 0) return blockingProjectilesObstacles.First();
        return null;
    }

    public void Clear() {
        new List<BaseZombie>(zombies).ForEach(z => DestroyImmediate(z.gameObject));
        zombies.Clear();
        new List<BaseObstacle>(obstacles).ForEach(o => DestroyImmediate(o.gameObject));
        obstacles.Clear();
    }
}
