using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Tile tile = TileManager.Instance.GetTileType(BlockType.Solar);
public class TileManager : MonoBehaviour
{
    // Create a static reference to the instance of TileManager
    public static TileManager Instance;
    public Tile plainTile;
    public Tile voidTile;
    public Tile solarTile;
    public Tile iceTile;
    public Tile prismaticTile;

    public Tile plainAlphaTile;
    public Tile voidAlphaTile;
    public Tile solarAlphaTile;
    public Tile iceAlphaTile;


    private void Awake()
    {
       if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public Tile GetTileType(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Plain => plainTile,
            BlockType.Void => voidTile,
            BlockType.Solar => solarTile,
            BlockType.Ice => iceTile,
            BlockType.Prismatic => prismaticTile,
            _ => null,
        };
    }

    public Tile GetAlphaTileType(BlockType blockType){
        return blockType switch
        {
            BlockType.Plain => plainAlphaTile,
            BlockType.Void => voidAlphaTile,
            BlockType.Solar => solarAlphaTile,
            BlockType.Ice => iceAlphaTile,
            _ => null,
        };
    }

}
