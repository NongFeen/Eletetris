using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI EndText;
    public TextMeshProUGUI ScoreText;
    public void Setup(int Stage, EnemyBase enemy, bool isWin){
        this.gameObject.SetActive(true);
        int curStage = Stage;
        if(isWin){
            EndText.text = "You Win";
            ScoreText.text= "Good Game You've beat all the enemy";
        }else{
            EndText.text = "GAME OVER";
            ScoreText.text = "Defeated by\n" + $"{enemy.Name}"+ "\nAt Stage : " + curStage;
        }
        ResetPref();
    }
    public void RestartButton(){
        SceneManager.LoadScene("Scenes/GameScene");
    }
    public void MainMenuButton(){
        SceneManager.LoadScene("Scenes/MainMenu");
    }
    private void ResetPref(){
        PlayerPrefs.SetInt("Stage",1);
    }
}
