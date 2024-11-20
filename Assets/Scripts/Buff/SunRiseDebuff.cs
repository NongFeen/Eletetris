using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRiseDebuff : GameBuff
{
    private float AmplifyValue = 0.3f;
    private bool Amplified = false;
    public override void SetDefaults()
    {
        BuffName = "Sun rise";
        BuffInfo = $"Ampify all damage with {AmplifyValue*100:00}% ";
        isActive = true;
        BuffSprite = Resources.Load<Sprite>("BuffIcon/SunRiseDebuff");
    }
    public override void UpdateBuff(EnemyBase enemy)
    {
        if (isActive&& !Amplified)
        {
            enemy.AmplifyDamage *=1+AmplifyValue;
            Amplified = true; 
        }
        base.UpdateBuff(enemy);
    }
    public override void OnRemove(EnemyBase enemy)
    {
        Amplified = false; 
        enemy.AmplifyDamage /=1+AmplifyValue;
    }
}
