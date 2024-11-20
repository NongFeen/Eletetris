using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class EnemyBase 
{
    public string Name;
    public float MaxHP;
    public float CurrentHP;
    public float[] DefenseStats; //Plain,Void,Solar,Ice
    public bool IsBoss;
    public Sprite EnemySprite;
    public BlockType ArmorType;

    public float AmplifyDamage =1;

    public EnemyBase()
    {
        SetDefaults();
        CurrentHP = MaxHP;
    }
    public virtual void SetDefaults()
    {
        Name = "Default Enemy";
        MaxHP = 100;
        DefenseStats = new float[] { 10, 10, 10, 10 };//Plain,Void,Solar,Ice
        EnemySprite = null;
        IsBoss = false;
        ArmorType = BlockType.Plain;
        AmplifyDamage = 1.0f;
    }
    public virtual void OnHit(float dmg, BlockType dmgType){
        OnTakeDamage(dmg,dmgType);
    }
    public virtual void OnTakeDamage(float damage, BlockType damageType)
    {
        if(damageType == ArmorType && damageType != BlockType.Plain)
            { damage *=0.5f;}
        float actualDamage = (damage *(1- DefenseToDmgReduction(damageType))) * AmplifyDamage;
        CurrentHP -= actualDamage; 
        // Debug.Log($"Damage Recived : {actualDamage}");
        if (IsDefeated())
        {
            OnDefeated();
        }
    }

    private float DefenseToDmgReduction(BlockType blockType){
        //dota 2 defense
        return (0.01f*DefenseStats[(int)blockType])/(1.0f+(0.01f*DefenseStats[(int)blockType]));;
        
    }
    public bool IsDefeated() => CurrentHP <= 0;
    public virtual void OnDefeated(){
    }
    public virtual void OnSpawn(){
        
        int stage = PlayerPrefs.GetInt("Stage");
        MaxHP *= 1 + 0.1f*stage;
        CurrentHP = MaxHP;
        Debug.Log($"Spawned {Name} (Boss: {IsBoss}) (MaxHP: {MaxHP}) Stage: {PlayerPrefs.GetInt("Stage")}");
    }
}
