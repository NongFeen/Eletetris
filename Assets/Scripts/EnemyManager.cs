using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public EnemyBase CurrentEnemy;
    public TextMeshProUGUI TextEnemyName;
    public TextMeshProUGUI TextEnemyHealth;
    public TextMeshProUGUI[] TextDefense;
    public SpriteRenderer spriteRender;
    public List<GameBuff> ActiveDebuffs = new List<GameBuff>();
    public GameObject BuffDisplay;
    public GameObject BuffPrefab;
    public GameObject EnemyHealthBar;
    public void SpawnEnemy<T>() where T : EnemyBase, new()
    {
        RemoveDebuff();
        CurrentEnemy = new T();
        CurrentEnemy.OnSpawn();
    }
    void Update(){
        DisplayStat();
        for (int i = ActiveDebuffs.Count - 1; i >= 0; i--)
        {
            GameBuff buff = ActiveDebuffs[i];
            buff.UpdateBuff(CurrentEnemy);
            // Remove expired buffs
            if (buff.Duration <= 0)
            {
                ActiveDebuffs.RemoveAt(i);
            }
        }
        DisplayBuff();
        UpdateHealthBar();
    }
    void DisplayStat(){
        if(CurrentEnemy != null){
            TextEnemyName.text = $"{CurrentEnemy.Name}";
            TextEnemyHealth.text = $"{CurrentEnemy.CurrentHP:0.00} / {CurrentEnemy.MaxHP}";
            for(int i=0;i<TextDefense.Length;i++){
                TextDefense[i].text = $"{CurrentEnemy.DefenseStats[i]}";
            }
            // TextDefense.text = $"Def\n Plain : {CurrentEnemy.DefenseStats[0]} \t Void : {CurrentEnemy.DefenseStats[1]}\n "+
            //                 $"Solar : {CurrentEnemy.DefenseStats[2]} \t Ice : {CurrentEnemy.DefenseStats[3]}";
            spriteRender.sprite = CurrentEnemy.EnemySprite;
        }else
            Debug.Log("Found No Enemy");
    }
    public void OnTakeDamage(float damage, BlockType dmgType){
        float actualDamage = CalculateDamage(damage,dmgType);
        CurrentEnemy.OnHit(actualDamage,dmgType);
    }
    public float CalculateDamage(float damageRecived,BlockType dmgType){
        return damageRecived;
    }
    public bool IsDead(){
        return CurrentEnemy.IsDefeated();
    }
    public void OnEnemyDead(){
        CurrentEnemy.OnDefeated();
    }
    
    public void AddBuff<T>(float Duration) where T : GameBuff, new()
    {
        T buffInstance = new T();
        buffInstance.SetDefaults();
        buffInstance.Duration = Duration;
        buffInstance.isActive = true;
        // Add to the active buffs list
        ActiveDebuffs.Add(buffInstance);
        // Debug.Log($"Buff {buffInstance.BuffName} added for {Duration} seconds.");
    }
    private void DisplayBuff(){
        foreach (Transform child in BuffDisplay.transform)
        {
            Destroy(child.gameObject);
        }
        //display each buff
        //not effective but it work!!
        foreach (GameBuff buff in ActiveDebuffs)
        {
            GameObject buffInstance = Instantiate(BuffPrefab, BuffDisplay.transform);
            buffInstance.GetComponent<SpriteRenderer>().sprite = buff.BuffSprite;
        }
    }
    public void UpdateHealthBar(){
        Slider slider = EnemyHealthBar.GetComponent<Slider>();
        float percentHealth = CurrentEnemy.CurrentHP / CurrentEnemy.MaxHP;
        slider.value = percentHealth;
        //turn green to red by 
        // increase color R to 255 will get yellow 50%
        //then green to 0
        UnityEngine.UI.Image image = slider.targetGraphic.GetComponent<UnityEngine.UI.Image>();
        float r = Mathf.Lerp(0, 255, 1 - (percentHealth-0.5f)*2f) / 255f;
        float g = Mathf.Lerp(255, 0, 1 - percentHealth*2) / 255f;
        float b = 0f;
        image.color = new Color(r, g, b);
    }
    public void RemoveDebuff(){
        ActiveDebuffs = new List<GameBuff>();
    }
}
