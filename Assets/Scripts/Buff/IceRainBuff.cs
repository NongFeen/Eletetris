using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRainBuff : GameBuff
{
    private float DamagePerSecond = 30;
    private float timeSinceLastDamage = 0f;

    public override void SetDefaults()
    {
        BuffName = "Ice Rain";
        BuffInfo = $"More likly to spawn Ice block% ";
        isActive = true;
        BuffSprite = Resources.Load<Sprite>("BuffIcon/IceRainBuff");
    }
    public override void UpdateBuff(EnemyBase enemy)
    {  
        //heavy rain does Ice dmg
        if (isActive && Duration > 7)
        {   
            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage >= 1f)
            {
                enemy.OnHit(DamagePerSecond,BlockType.Solar);
                timeSinceLastDamage -= 1f;
            }
        }
        base.UpdateBuff(enemy);
    }
}