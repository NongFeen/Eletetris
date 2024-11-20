using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltingDebuff : GameBuff
{
    private float ArmorShredPercent = 0.2f;
    private bool armorShredded = false;
    public override void SetDefaults()
    {
        BuffName = "Melting";
        BuffInfo = $"Reduce Solar armor for {ArmorShredPercent*100:00} ";
        isActive = true;
        BuffSprite = Resources.Load<Sprite>("BuffIcon/MeltingDebuff");
    }
    public override void UpdateBuff(EnemyBase enemy)
    {
        if (isActive&& !armorShredded)
        {
            enemy.DefenseStats[(int)BlockType.Solar] *= 1-ArmorShredPercent;
            armorShredded = true; 
        }
        base.UpdateBuff(enemy);
    }
    public override void OnRemove(EnemyBase enemy)
    {
        armorShredded = false; 
        enemy.DefenseStats[(int)BlockType.Solar] /= 1-ArmorShredPercent;
    }
}
