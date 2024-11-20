using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBreakDebuff : GameBuff
{
    private float ArmorShredPercent = 0.8f;
    private bool armorShredded = false;
    public override void SetDefaults()
    {
        BuffName = "Shield Break";
        BuffInfo = $"Remove 80% of armor";
        isActive = true;
        BuffSprite = Resources.Load<Sprite>("BuffIcon/ShieldBreakDebuff");
    }
    public override void UpdateBuff(EnemyBase enemy)
    {
        if (isActive&& !armorShredded)
        {
            enemy.DefenseStats[(int)BlockType.Solar] *= 1-ArmorShredPercent;
            enemy.DefenseStats[(int)BlockType.Void] *= 1-ArmorShredPercent;
            enemy.DefenseStats[(int)BlockType.Ice] *= 1-ArmorShredPercent;
            enemy.DefenseStats[(int)BlockType.Plain] *= 1-ArmorShredPercent;
            armorShredded = true; 
        }
        base.UpdateBuff(enemy);
    }
    public override void OnRemove(EnemyBase enemy)
    {
        armorShredded = false; 
        enemy.DefenseStats[(int)BlockType.Solar] /= 1-ArmorShredPercent;
        enemy.DefenseStats[(int)BlockType.Void] /= 1-ArmorShredPercent;
        enemy.DefenseStats[(int)BlockType.Ice] /= 1-ArmorShredPercent;
        enemy.DefenseStats[(int)BlockType.Plain] /= 1-ArmorShredPercent;
    }
}
