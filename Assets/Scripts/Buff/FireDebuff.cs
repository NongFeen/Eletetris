using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDebuff : GameBuff
{
    private float DamagePerSecond = 10;
    private float timeSinceLastDamage = 0f;
    public override void SetDefaults()
    {
        BuffName = "Burn";
        BuffInfo = $"Recived {DamagePerSecond} fire damage per second";
        isActive = true;
        BuffSprite = Resources.Load<Sprite>("BuffIcon/FireDebuff");
    }
    public override void UpdateBuff(EnemyBase enemy)
    {
        if (isActive)
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
