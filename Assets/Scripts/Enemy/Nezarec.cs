using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nezarec : EnemyBase
{
    public override void SetDefaults()
    {
        Name = "Nezarec, Final God";
        MaxHP = 1500;
        DefenseStats = new float[] { 50, 100, 100, 100 };
        EnemySprite = Resources.Load<Sprite>("Enemy/Nezarec");
        IsBoss = true;
        ArmorType = BlockType.Plain;
    }
    public override void OnHit(float dmg, BlockType dmgType)
    {
        for(int i=0;i<DefenseStats.Length;i++){
            if(i==(int)dmgType){
                DefenseStats[i] +=10;
            }
            else
                DefenseStats[i] +=5;
        base.OnHit(dmg, dmgType);
        }
    }
}
