using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BoardControl : MonoBehaviour
{
    //spawn Tetromino
    public Tilemap tilemap;
    public TetrominoMovePiece MovePiece;
    public Vector3Int spawnPos;
    public BlockType MainType;
    public EnemyManager Enemy;
    public TextMeshProUGUI DebugLastElemental;
    public SpriteRenderer LastDamageTypeSpriteRender;
    public TextMeshProUGUI LastAttackDamage;
    public SpriteRenderer Character;
    private BlockType DamageType;
    public GameOver gameOver;
    public bool isGameOver;
    private readonly int EnemyCount =4;
    private readonly int MAX_Row = 20,MAX_Column =10;
    private int[] LastElementCellClear = new int[5];//Plain V Solar I Prismatic

    void Start(){
        isGameOver =false;
        UpdateCharacter();
        SpawnTetrimino();
        // for(int i =0; i< 9;i++){
        //     tilemap.SetTile(new Vector3Int(i,0),TileManager.Instance.GetTileType(MainType));
        //     tilemap.SetTile(new Vector3Int(i,1),TileManager.Instance.GetTileType(MainType));
        //     tilemap.SetTile(new Vector3Int(i,2),TileManager.Instance.GetTileType(MainType));
        //     tilemap.SetTile(new Vector3Int(i,3),TileManager.Instance.GetTileType(MainType));
        //     // tilemap.SetTile(new Vector3Int(i,4),TileManager.Instance.GetTileType(MainType));
        // }
        // tilemap.SetTile(new Vector3Int(0,5), TileManager.Instance.GetTileType(BlockType.Prismatic));
        // SpawnRandomEnemy();
        SpawnEnemyInOrder();
    }
    void Update(){
        if (Enemy.IsDead()&&!isGameOver)
        {
            Enemy.OnEnemyDead();
            Debug.Log("Enemy Is Dead, Current Stage :" + PlayerPrefs.GetInt("Stage"));
            //spawn next enemy
            if(IsDefeatAll()){
                OnGameWin();
            }else{
                Debug.Log($"Increase {PlayerPrefs.GetInt("Stage")} to {PlayerPrefs.GetInt("Stage")+1}");
                PlayerPrefs.SetInt("Stage", PlayerPrefs.GetInt("Stage")+1);
                SpawnEnemyInOrder();
            }
            // SpawnRandomEnemy();
        }
    }
    public void SpawnTetrimino()
    {
        Debug.Log("Spawning new Tetromino...");
        TetrominoShape shape = RandomTetriminoShape();
        DamageType = shape.GetBlockType();
        if (!IsValidPositionAtSpawn(shape)&& isGameOver == false)
        {
            Debug.Log("Game Over! Cannot spawn a new Tetromino.");
            OnGameOver();
            return;
        }
        MovePiece.CreateMovePiece(this, spawnPos, shape);
    }
    private void OnGameOver(){
        if(!isGameOver){
            Debug.Log("Game OVER");
            gameOver.Setup(PlayerPrefs.GetInt("Stage"),Enemy.CurrentEnemy,false);
            isGameOver =true;
        }
    }
    private void OnGameWin(){
        if(!isGameOver){
            Debug.Log("Good Game");
            gameOver.Setup(PlayerPrefs.GetInt("Stage"),Enemy.CurrentEnemy,true);
            isGameOver =true;
        }
    }
    private bool IsValidPositionAtSpawn(TetrominoShape shape)
    {
        foreach (Vector2Int cell in shape.cellsPos)
        {
            Vector3Int cellPos = spawnPos + new Vector3Int(cell.x, cell.y, 0);
            if (tilemap.GetTile(cellPos) != null && tilemap.GetColor(cellPos).a == 1.0f)
            {
                return false; 
            }
        }
        return true;
    }
    TetrominoShape RandomTetriminoShape()
    {
        ShapeType randomType = (ShapeType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ShapeType)).Length);
        ShapeData shapeData = ShapeManager.Instance.GetShape(randomType);
        BlockType dmgtype = RandomTypeWithWeight(MainType);
        if(HasIceRainBuff()){
            dmgtype = BlockType.Ice;
        }
        // ShapeType Ipiece = ShapeType.I;
        // ShapeData shapeData = ShapeManager.Instance.GetShape(Ipiece);
        TetrominoShape shape = new TetrominoShape(shapeData.cellsPos, dmgtype,shapeData.pivot,shapeData.canRotate);
        // shape.CreateShape(shapePos, RandomTypeWithWeight(MainType));
        
        return shape;
    }
    public void SaveCell(TetrominoMovePiece piece){
        //save each cell to tilemap
        // Debug.Log("Saving Cell " + piece);
        TetrominoShape shape = piece.CurrentShape;
        Tile type = TileManager.Instance.GetTileType(shape.GetBlockType());
    
        foreach (Vector2Int cell in shape.cellsPos)
        {
            Vector3Int cellPos = piece.CurrentPos + new Vector3Int(cell.x, cell.y, 0);
            tilemap.SetTile(cellPos, type);
            // Debug.Log("Save cell at " + "X :" + cellPos.x + "Y" + cellPos.y);
        }
    }
    public bool HasIceRainBuff(){
        foreach (var buff in Enemy.ActiveDebuffs)
        {
            if(buff is IceRainBuff){
                return true;
            }
        }
        return false;
    }
    BlockType RandomTypeWithWeight(BlockType MainType){
        // 4:2:2:2
        int rnd = UnityEngine.Random.Range(0, 10);
        // Debug.Log(rnd);
        switch (rnd)
        {
            case 0:
            case 1:
                return BlockType.Plain;
            case 2:
            case 3:
                return BlockType.Void;
            case 4:
            case 5:
                return BlockType.Solar;
            case 6:
            case 7:
                return BlockType.Ice; 
            default:
                return MainType;
        }
    }
    public void ClearLines(){
        int ClearLineCount =0;
        int[] cellCountByType = new int[4];
        int LastClearedRow = MAX_Row+1;

        for(int row=0;row<MAX_Row;row++){
            //check from 0 to 20 (bot to top)
            if(IsFullLine(row)){
                //count each cell
                CategorizeCell(row);

                //delete line 
                DeleteLine(row);
                ClearLineCount +=1;
                //find lowlest row that got cleared
                if(row<LastClearedRow){
                    LastClearedRow = row;
                }
                //shift other down by ClearLineCount
                ShiftTilesDown(row);
                //recheck same row
                row-=1;
            }
        }
        DebugLastElemental.text = $"Last Type : {DamageType}   \n Lowest: {LastClearedRow}  Line: {ClearLineCount}\n"+"Plain : " + LastElementCellClear[0] + " \t Solar : " 
        + LastElementCellClear[1]+ "\nVoid : " + LastElementCellClear[2] + " \t Ice : " + LastElementCellClear[3] + "\nPrismatic" + LastElementCellClear[4];
        
        //spawn prismatic block 
        if (CanSpawnPrismaticBlock(ClearLineCount,LastClearedRow))
        {
            SpawnPrismaticBlock(FindPrismaticSpawn(LastClearedRow));
        }

        //dmg 
        float CalDamage = CalculateTileDamage(ClearLineCount);
        if(CalDamage <=0){
            Debug.Log("Immune");
            return;
        }else{
            UpdateLastDamageDisplay(CalDamage,DamageType);
            Enemy.OnTakeDamage(CalDamage,DamageType);
            AppileExtraAttack(ClearLineCount);
            //do effect
        }
        ResetCellCount();
    }
    bool IsFullLine(int row){
        for(int i=0;i<MAX_Column;i++){
            if(tilemap.GetTile(new Vector3Int(i,row,0))==null || tilemap.GetColor(new Vector3Int(i,row,0)).a !=1.0f){
                return false;
            }
            // Debug.Log("Get Tile at X = "+i + ", Y = " + row + "Tile =" + tilemap.GetTile(new Vector3Int(i,row,0)).ToString());
        }
        return true;
    }
    private void CategorizeCell(int row){
        for(int i =0;i< MAX_Column;i++){
            TileBase tile = tilemap.GetTile(new Vector3Int(i,row,0));
            if (tile != null && tile)
            {
                string tileName = tile.ToString().Split(' ')[0];
                switch (tileName)
                {
                    case "PlainBlock":
                        LastElementCellClear[0]++;
                        break;
                    case "VoidBlock":
                        LastElementCellClear[1]++;
                        break;
                    case "SolarBlock":
                        LastElementCellClear[2]++;
                        break;
                    case "IceBlock":
                        LastElementCellClear[3]++;
                        break;
                    case "PrismaticBlock":
                        LastElementCellClear[4]++;
                        break;
                }
            }
        }
    }
    private void DeleteLine(int row){
        for (int col = 0; col < MAX_Column; col++)
        {
            Vector3Int cellPos = new Vector3Int(col, row, 0);
            tilemap.SetTile(cellPos, null); // Remove tile from the row
        }
    }
    private void ShiftTilesDown(int startRow)
    {
        for (int row = startRow + 1; row < MAX_Row; row++) // Start from the row above
        {
            for (int col = 0; col < MAX_Column; col++)
            {
                Vector3Int aboveCell = new Vector3Int(col, row, 0);
                Vector3Int belowCell = new Vector3Int(col, row - 1, 0);

                TileBase tileAbove = tilemap.GetTile(aboveCell);
                if (tileAbove != null)
                {
                    tilemap.SetTile(belowCell, tileAbove); // Move tile down
                    tilemap.SetTile(aboveCell, null);     // Clear old position
                }
            }
        }
    }
    private void SpawnEnemy<T>() where T : EnemyBase, new()//for fixed spawn
    {
        if (Enemy == null)
        {
            return;
        }
        Debug.Log("Spawning enemy...");
        Enemy.SpawnEnemy<T>();
    }
    public void SpawnRandomEnemy(){
        // this is so hard
        // why......
        //...
        // oh just make switch case for all enemy 
        int random = UnityEngine.Random.Range(0, 3);
        switch (random)
        {
            case 0: 
                SpawnEnemy<AncientApparition>();
                break;
            case 1:
                SpawnEnemy<PhoenixBoss>();
                break;
            case 2: 
                SpawnEnemy<Nezarec>();
                break;
        }
    }
    public void SpawnEnemyInOrder(){
        Debug.Log("Spawning Enemy Inorder at Stage : "+ PlayerPrefs.GetInt("Stage"));
        switch (PlayerPrefs.GetInt("Stage"))
        {
            case 1: 
                SpawnEnemy<AncientApparition>();
                break;
            case 2:
                SpawnEnemy<PhoenixBoss>();
                break;
            case 3: 
                SpawnEnemy<Nezarec>();
                break;
        }
    }
    private bool IsDefeatAll(){
        if(PlayerPrefs.GetInt("Stage") == EnemyCount){
            return true;
        }
        return false;
    }
    public float CalculateTileDamage(int lineCount){
        float totaldmg =0.0f;
        float multiplier = GetLineMultiplier(lineCount);
        //plain Solar void Ice prismatic
        foreach (int count in LastElementCellClear)
        {
            if(DamageType == MainType)//4 dmg per cell 
                totaldmg += 4*count;
            else 
                totaldmg += 3*count;//3 dmg per cell
        }
        return totaldmg*multiplier;
    }
    private float GetLineMultiplier(int lineCount){
        return lineCount switch 
        {
            1 => 1.0f,
            2 => 1.5f,
            3 => 2.0f,
            4 => 2.5f,
            5 => 4.0f,
            6 => 6.0f,
            7 => 9.0f,
            8 => 13.0f,
            _ => 20.0f,
        };
    }
    public void ResetCellCount(){
        for(int i =0;i<LastElementCellClear.Length;i++){
            LastElementCellClear[i]=0;
        }
    }
    private bool CanSpawnPrismaticBlock(int ClearLineCount,int LastClearedRow){
        return ClearLineCount >= 4 && LastClearedRow >= 0;
    }
    private int[] FindPrismaticSpawn(int LastClearedRow){
        int[] pos = new int[2];
        pos[1] = LastClearedRow;
        for(int i=0;i<MAX_Column;i++){
            if(tilemap.GetTile(new Vector3Int(i,LastClearedRow,0))==null){
                pos[0] =i;
                break;
            }
        }
        return pos;
    }
    private void SpawnPrismaticBlock(int[] pos)
    {
        Vector3Int prismaticBlockPosition = new Vector3Int(pos[0], pos[1], 0);
        TileBase prismaticTile = TileManager.Instance.GetTileType(BlockType.Prismatic);
        tilemap.SetTile(prismaticBlockPosition, prismaticTile);
    }
    private void UpdateLastDamageDisplay(float damage,BlockType dmgType){
        LastDamageTypeSpriteRender.sprite = GetSpriteFromDamageType(dmgType);
        LastAttackDamage.text = $"{damage:0.00}";
    }
    private Sprite GetSpriteFromDamageType(BlockType dmgType){
        return dmgType switch
        {
            BlockType.Plain => Resources.Load<Sprite>("Sprite/PlainBlock"),
            BlockType.Void => Resources.Load<Sprite>("Sprite/VoidBlock"),
            BlockType.Solar => Resources.Load<Sprite>("Sprite/SolarBlock"),
            BlockType.Ice => Resources.Load<Sprite>("Sprite/IceBlock"),
            BlockType.Prismatic => Resources.Load<Sprite>("Sprite/PrismaticBlock"),
            _ => Resources.Load<Sprite>("Sprite/PlainBlock"),
        };
    }
    private void UpdateCharacter(){
        BlockType Class = (BlockType)PlayerPrefs.GetInt("Class");
        Character.sprite =Class switch
        {
            BlockType.Void => Resources.Load<Sprite>("Enemy/DarkSeer"),
            BlockType.Solar => Resources.Load<Sprite>("Enemy/Lina"),
            BlockType.Ice => Resources.Load<Sprite>("Enemy/CrystalMaiden"),
            _ => Resources.Load<Sprite>("Sprite/PlainBlock"),
        };
        MainType = Class;
    }
    private void AppileExtraAttack(int ClearLineCount){
        // Debug.Log($"{DamageType}");
        switch (DamageType)
        {
            case BlockType.Plain:
                switch (ClearLineCount)
                {
                    case 1:
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 2:
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 3:
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 4:
                        AudioManager.Instance.PlaySFX(3);
                        break;
                    case 5:
                        break;
                }
                    break;
            case BlockType.Void:
                switch (ClearLineCount)
                {
                    case 1:
                        Enemy.AddBuff<WeakenDebuff>(15);
                        AudioManager.Instance.PlaySFX(7);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 2:
                        // Debug.Log("Void ");
                        Enemy.OnTakeDamage(150,BlockType.Void);
                        if(MainType == BlockType.Void){//echo
                            Enemy.OnTakeDamage(200,BlockType.Void);
                        }
                        AudioManager.Instance.PlaySFX(8);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 3:
                        Enemy.AddBuff<ShieldBreakDebuff>(30);
                        AudioManager.Instance.PlaySFX(7);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 4:
                        Enemy.AddBuff<VulnerableDebuff>(60);
                        AudioManager.Instance.PlaySFX(7);
                        AudioManager.Instance.PlaySFX(3);
                        break;
                    case 5:
                        Enemy.AddBuff<WeakenDebuff>(30);
                        Enemy.AddBuff<ShieldBreakDebuff>(30);
                        Enemy.AddBuff<VulnerableDebuff>(30);
                        AudioManager.Instance.PlaySFX(7);
                        break;
                }
                break;
            case BlockType.Solar:
                // Debug.Log($"Solar {ClearLineCount}");
                switch (ClearLineCount)
                {
                    case 1:
                        // Debug.Log("Apply fire debuff");
                        Enemy.AddBuff<FireDebuff>(LastElementCellClear[2]*2);
                        AudioManager.Instance.PlaySFX(6);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 2:
                        // Debug.Log("Apply MeltingDebuff");
                        Enemy.AddBuff<MeltingDebuff>(15);
                        AudioManager.Instance.PlaySFX(6);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 3:
                        // Debug.Log("Apply Sun Rise");
                        Enemy.AddBuff<SunRiseDebuff>(30);
                        AudioManager.Instance.PlaySFX(6);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 4://attck again with fire dmg
                        // Debug.Log("Leguna Blade");
                        Enemy.OnTakeDamage(LastElementCellClear[2]*30,BlockType.Solar);
                        AudioManager.Instance.PlaySFX(5);//laguna blade sound
                        AudioManager.Instance.PlaySFX(3);
                        break;
                    case 5:
                        Enemy.AddBuff<FireDebuff>(120);
                        Enemy.AddBuff<MeltingDebuff>(60);
                        Enemy.AddBuff<SunRiseDebuff>(60);
                        AudioManager.Instance.PlaySFX(6);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                }
                break;
            case BlockType.Ice:
                switch (ClearLineCount)
                    {
                    case 1:
                        Enemy.OnTakeDamage(40,BlockType.Ice);
                        if(MainType == BlockType.Ice){
                            Enemy.OnTakeDamage(Enemy.CurrentEnemy.CurrentHP/50.0f,BlockType.Ice);
                        }
                        AudioManager.Instance.PlaySFX(10);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 2:
                        Enemy.AddBuff<IceRainBuff>(3);
                        AudioManager.Instance.PlaySFX(9);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 3:
                        // Debug.Log("Ice Rain");
                        Enemy.AddBuff<IceRainBuff>(10);
                        AudioManager.Instance.PlaySFX(9);
                        AudioManager.Instance.PlaySFX(4);
                        break;
                    case 4:
                        int ice = 10;
                        for(int x=0;x<MAX_Column;x++){
                            for(int y=0;y<MAX_Row;y++){
                                if(tilemap.GetTile(new Vector3Int(x,y,0))== TileManager.Instance.GetTileType(BlockType.Ice)){
                                    ice++;
                                }
                            }
                        }
                        Enemy.OnTakeDamage(ice*20,BlockType.Ice);
                        AudioManager.Instance.PlaySFX(3);
                        break;
                    case 5:
                        AudioManager.Instance.PlaySFX(9);
                        Enemy.AddBuff<IceRainBuff>(60);
                        Enemy.OnTakeDamage(Enemy.CurrentEnemy.CurrentHP/2.0f,BlockType.Ice);
                        int ices = 10;
                        for(int x=0;x<MAX_Column;x++){
                            for(int y=0;y<MAX_Row;y++){
                                if(tilemap.GetTile(new Vector3Int(x,y,0))== TileManager.Instance.GetTileType(BlockType.Ice)){
                                    ices++;
                                }
                            }
                        }
                        Enemy.OnTakeDamage(ices*30,BlockType.Ice);
                        break;
                    }
                break;
            case BlockType.Prismatic:
                //never happening
                break;
        }
    }
}
