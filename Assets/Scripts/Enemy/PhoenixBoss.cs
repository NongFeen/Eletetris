using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixBoss : EnemyBase
{
    public override void SetDefaults()
    {
        Name = "Phoenix";
        MaxHP = 700;
        DefenseStats = new float[] { 50, 100, 999, 0 };
        EnemySprite = Resources.Load<Sprite>("Enemy/PhoenixBoss");
        IsBoss = true;
        ArmorType = BlockType.Solar;
    }
    public override void OnHit(float dmg, BlockType dmgType)
    {
        if(dmgType == ArmorType){
            dmg =0;
        }else if(dmgType == BlockType.Ice){
            dmg *=2;
        }
        base.OnHit(dmg, dmgType);
    }
}
