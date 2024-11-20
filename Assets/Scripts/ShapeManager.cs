using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager Instance;
    public Dictionary<ShapeType, ShapeData> Shapes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        SetUpShape();
    }
    //The J, L and T spawn flat-side first, while I, S and Z spawn in their upper horizontal orientation. 
    //In Tetris Worlds, the tetrominoes spawn in rows 22 and 23
    private void SetUpShape()
    {
        // Initialize shapes dictionary
        Shapes = new Dictionary<ShapeType, ShapeData>();
        Shapes.Add(ShapeType.I, new ShapeData
        {
            // oxoo
            cellsPos =  new Vector2Int[]{    //
            new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1) //I shape
            },
            pivot = new Vector2Int(1,0),
            canRotate = true
        });
        Shapes.Add(ShapeType.L, new ShapeData
        {
            //   0
            // oxo
            cellsPos =  new Vector2Int[]{    
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1) //L shape
            },
            pivot = new Vector2Int(1,0),
            canRotate = true
        });
        Shapes.Add(ShapeType.J, new ShapeData
        {
            // o 
            // oxo
            cellsPos =  new Vector2Int[]{    
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1) //L shape
            },
            pivot = new Vector2Int(1,0),
            canRotate = true
        });
        Shapes.Add(ShapeType.T, new ShapeData
        {
            //  o
            // oxo
            cellsPos =  new Vector2Int[]{    
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, 1) //L shape
            },
            pivot = new Vector2Int(1,0),
            canRotate = true
        });
        Shapes.Add(ShapeType.O, new ShapeData
        {
            //  o
            // oxo
            cellsPos =  new Vector2Int[]{    
            new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1) //Square shape
            },
            pivot = new Vector2Int(0,0),
            canRotate = false
        });
        Shapes.Add(ShapeType.S, new ShapeData
        {
            //  oo
            // ox
            cellsPos =  new Vector2Int[]{    
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) //S shape
            },
            pivot = new Vector2Int(1,0),
            canRotate = true
        });
        Shapes.Add(ShapeType.Z, new ShapeData
        {
            // oo
            //  xo
            cellsPos =  new Vector2Int[]{    
            new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 0) //S shape
            },
            pivot = new Vector2Int(1,0),
            canRotate = true
        });
        Shapes.Add(ShapeType.Dot, new ShapeData
        {
            cellsPos =  new Vector2Int[]{
                new Vector2Int(0, 0)
            },
            pivot = new Vector2Int(0, 0),
            canRotate =false
        });
    }

    // Method to get a shape by type
    public ShapeData GetShape(ShapeType type)
    {
        if (Shapes.ContainsKey(type))
            return Shapes[type];
        // Debug.LogError($"Shape {type} not found in ShapeManager!");
        return default;
    }


}
