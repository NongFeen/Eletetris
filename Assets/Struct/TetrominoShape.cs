using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct TetrominoShape
{
    //Store how Tetromino default spawn shape can  have : i l j t o s z
    public BlockType type;
    public Vector2Int[] cellsPos;
    public Vector2Int pivot;
    public bool CanRotate;
    public TetrominoShape(Vector2Int[] pos, BlockType element,Vector2Int pivotPos,bool canRotate){
        cellsPos = pos;
        type = element;
        pivot = pivotPos;
        CanRotate = canRotate;
    }
    public void CreateShape(Vector2Int[] pos,BlockType element){
        cellsPos = pos;
        type = element;
    }
    public BlockType GetBlockType(){
        return this.type;
    }
    public void SetBlockType(BlockType newType){
        this.type = newType;
    }
}

