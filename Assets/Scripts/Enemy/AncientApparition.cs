using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientApparition : EnemyBase
{
    public override void SetDefaults()
    {
        Name = "Ancient Apparition";    
        MaxHP = 750;
        DefenseStats = new float[] { 30, 10, 0, 999 };
        EnemySprite = Resources.Load<Sprite>("Enemy/AncientApparition");
        IsBoss = false;
        ArmorType = BlockType.Ice;
    }
    public override void OnHit(float dmg, BlockType dmgType)
    {
        if(dmgType == BlockType.Ice){
            dmg =0;
        }
        base.OnHit(dmg, dmgType);
    }
}
