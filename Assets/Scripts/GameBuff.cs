using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBuff 
{
    public float Duration;
    public bool isActive;         
    public string BuffName;
    public string BuffInfo { get; set; }
    public Sprite BuffSprite { get; set; }

    public virtual void SetDefaults()
    {
        BuffName = "default Name";
        BuffInfo = "Buff Info";
        isActive = true;
        BuffSprite = null;
    }
    public virtual void OnApplyBuff(EnemyBase enemy){

    }
    public virtual void UpdateBuff(EnemyBase enemy){
        Duration -= Time.deltaTime;
        if (this.Duration <= 0)
            {
                isActive= false; // Remove the debuff when it expires
                OnRemove(enemy);
            }
    }
    public virtual void OnRemove(EnemyBase enemy)
    {
        isActive = false;
    }
    public virtual bool IsActive(){
        return isActive;
    }
}
