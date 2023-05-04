using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public Tile tilePrefab;
    public List<TileState> tileStates;
    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting = false;
    private void Awake() {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }
    public void ClearBoard(){
        foreach(var cell in grid.cells){
            cell.tile = null;
        }
        foreach(var tile in tiles){
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }
    public void CreateTile(){
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }
    private void Update() {
        if(!waiting)
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            } else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            } else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            } else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
    }
    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY){
        bool changed = false; 
        for(int x = startX; x >= 0 && x < grid.width; x += incrementX){
            for(int y = startY; y >= 0 && y < grid.height; y += incrementY){
                TileCell cell = grid.GetCell(x, y);
                if(cell.occupied)
                    changed |= MoveTile(cell.tile, direction);
            }
        }
        if(changed) StartCoroutine(WaitForChanges());
    }
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if(CanMerge(tile, adjacent.tile)){
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }
    private void Merge(Tile a, Tile b){
        tiles.Remove(a);
        a.Merge(b.cell);
        TileState newState = 
            tileStates.IndexOf(b.state) == tileStates.Count ? b.state : tileStates[tileStates.IndexOf(b.state) + 1];
        b.SetState(newState, (b.num*2) );
        gameManager.AddPoints(b.num*2);
    }

    private bool CanMerge(Tile a, Tile b){
        return a.num == b.num && !b.locked;
    }
    private IEnumerator WaitForChanges(){
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;
        foreach(var tile in tiles){
            tile.locked = false;
        }
        if(tiles.Count != grid.size) CreateTile();
        if(CheckForGameOver()) gameManager.GameOver();
    }
    private bool CheckForGameOver(){
        if(tiles.Count < grid.size) return false;
        foreach(var tile in tiles){
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);
            
            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }
        return true;
    }
}