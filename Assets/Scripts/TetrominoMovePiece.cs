using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TetrominoMovePiece : MonoBehaviour
{
    public BoardControl Currentboard;
    public Vector3Int CurrentPos;      // Current position of the Tetromino
    public TetrominoShape CurrentShape; // Shape of the Tetromino being moved
    public Vector2Int CurrentPivot;
    public bool CanRotate;
    private readonly float DelayDropTime = 1.5f;
    private float CurrentCountdown = 0.0f;
    public TextMeshProUGUI DebugText;
    public void CreateMovePiece(BoardControl board, Vector3Int pos, TetrominoShape shape)
    {
        Currentboard = board;
        CurrentPos = pos;
        CurrentShape = shape;
        CurrentPivot = shape.pivot;
        CanRotate = shape.CanRotate;
        DrawTetromino(CurrentPos);
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if ( Currentboard.isGameOver) return;
        ClearTetromino(CurrentPos);
        ClearAim();
        Movement();
        if(CanRotate)
            Rotate();
        AutoDropPiece();
        DrawHardDropAim();
        DrawTetromino(CurrentPos);
    }
    void SaveCell(){
        Currentboard.SaveCell(this);
    }
    void AutoDropPiece(){
        //somehow it work dont touch this thing again
        CurrentCountdown +=Time.deltaTime;
        if(DelayDropTime <= CurrentCountdown){
            Vector3Int newPos = CurrentPos + Vector3Int.down;
            if (IsValidPosition(newPos)) //autodrop
            {
                CurrentPos += Vector3Int.down;
            }
            else
            {
                LockPiece();
            }
            CurrentCountdown =0;
        }
        DebugText.text = "Current countdown" + CurrentCountdown ;
    }
    void LockPiece()
    {
        // DebugText.text = DebugText.text  + "\nLock at position:  + CurrentPos";
        Debug.Log("Locking Piece");
        CurrentCountdown =0;
        SaveCell();
        Currentboard.ClearLines();
        Currentboard.SpawnTetrimino();
        Debug.Log("Locked");
    }
    void DrawTetromino(Vector3Int position)
    {
        foreach (Vector2Int cell in CurrentShape.cellsPos)
        {
            Vector3Int cellPos = position + new Vector3Int(cell.x, cell.y, 0);
            Currentboard.tilemap.SetTile(cellPos, TileManager.Instance.GetTileType(CurrentShape.GetBlockType()));
        }
    }
    void ClearTetromino(Vector3Int position)
    {
        foreach (Vector2Int cell in CurrentShape.cellsPos)
        {
            Vector3Int cellPos = position + new Vector3Int(cell.x, cell.y, 0);
            Currentboard.tilemap.SetTile(cellPos, null);
        }
    }
    void Movement(){
        // Clear the Tetromino from the current position
        
        // movement input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SoftDrop();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AudioManager.Instance.PlaySFX(1);
            Vector3Int newPos = CurrentPos + Vector3Int.down;
            if (IsValidPosition(newPos)) {
                CurrentPos += Vector3Int.down;
                CurrentCountdown =0f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AudioManager.Instance.PlaySFX(1);
            Vector3Int newPos = CurrentPos + Vector3Int.left;
            if (IsValidPosition(newPos)) CurrentPos += Vector3Int.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AudioManager.Instance.PlaySFX(1);
            Vector3Int newPos = CurrentPos + Vector3Int.right;
            if (IsValidPosition(newPos)) CurrentPos += Vector3Int.right;
        }
    }
    void HardDrop()
    {
        CurrentPos = CalDropPos();
        AudioManager.Instance.PlaySFX(0);
        // then lock it
        LockPiece();
    }
    void SoftDrop()
    {
        CurrentPos = CalDropPos();
        CurrentCountdown = 0.0f;
        AudioManager.Instance.PlaySFX(0);
    }
    private void DrawHardDropAim(){
        //delete previous aim
        ClearAim();
        Vector3Int dropPos = CalDropPos();
        foreach (Vector2Int cell in CurrentShape.cellsPos)
        {
            Vector3Int cellPos = dropPos + new Vector3Int(cell.x, cell.y, 0);
            Currentboard.tilemap.SetTile(cellPos, TileManager.Instance.GetAlphaTileType(CurrentShape.GetBlockType()));
        }
    }
    private void ClearAim(){
        //delete previous aim
        Vector3Int dropPos = CalDropPos();
        foreach (Vector2Int cell in CurrentShape.cellsPos)
        {
            Vector3Int cellPos = dropPos + new Vector3Int(cell.x, cell.y, 0);
            Currentboard.tilemap.SetTile(cellPos, null);
        }
    }
    private Vector3Int CalDropPos(){
        Vector3Int dropPos = CurrentPos;
        // Move down until hit 
        while (IsValidPosition(dropPos + Vector3Int.down))
        {
            dropPos += Vector3Int.down;
        }
        return dropPos;
    }
    bool IsValidPosition(Vector3Int position)
    {
        int minX = 0;
        int maxX = 9;  
        int minY = 0;
        int maxY = 30;
        //look all cell in shape to check each position
        foreach (Vector2Int cell in CurrentShape.cellsPos)
        {
            Vector3Int cellPos = position + new Vector3Int(cell.x, cell.y, 0);
            if (cellPos.x < minX || cellPos.x > maxX || cellPos.y < minY || cellPos.y > maxY)
            {
                return false; 
            }
            if (Currentboard.tilemap.GetTile(cellPos) != null && Currentboard.tilemap.GetColor(cellPos).a == 1.0f)
                return false; 
            }
        return true;
    }
    void Rotate(){
        
        Vector2Int pivot = CurrentPivot; //pivot point base on shape 
        if (Input.GetKeyDown(KeyCode.A)) // CCW
        {
            AudioManager.Instance.PlaySFX(2);
            Vector2Int[] newPos = new Vector2Int[CurrentShape.cellsPos.Length];
            for (int i = 0; i < CurrentShape.cellsPos.Length; i++)
            {
                Vector2Int cell = CurrentShape.cellsPos[i];
                Vector2Int translated = cell - pivot; // Translate to (0, 0)
                Vector2Int rotated = new Vector2Int(-translated.y, translated.x);
                newPos[i] = rotated + pivot; // Translate back to the original position
            }
            if (IsValidRotate(newPos, CurrentPos)){
                CurrentShape.cellsPos = newPos;
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) 
        {
            AudioManager.Instance.PlaySFX(2);
            Vector2Int[] newPos = new Vector2Int[CurrentShape.cellsPos.Length];
            for (int i = 0; i < CurrentShape.cellsPos.Length; i++){
                Vector2Int cell = CurrentShape.cellsPos[i];
                Vector2Int translated = cell - pivot; 
                Vector2Int rotated = new Vector2Int(translated.y, -translated.x); 
                newPos[i] = rotated + pivot;
            }
            if (IsValidRotate(newPos, CurrentPos)){
                CurrentShape.cellsPos = newPos;
            }
        }
    }
    bool IsValidRotate(Vector2Int[] pos, Vector3Int shapePos){
        int minX = 0;
        int maxX = 9;  
        int minY = 0;
        int maxY = 30;
        for(int i=0;i< pos.Length;i++){
            Vector3Int cellPos = shapePos + new Vector3Int(pos[i].x, pos[i].y, 0);
            if (cellPos.x < minX || cellPos.x > maxX || cellPos.y < minY || cellPos.y > maxY){
                return false; 
            }
            if (Currentboard.tilemap.GetTile(cellPos) != null){
                return false;
            }
        }
        return true;
    }
}
