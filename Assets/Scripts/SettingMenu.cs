using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingMenu : MonoBehaviour
{
    public GameObject Menu;
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F10)){
            if(!Menu.activeSelf)
                Pause();
            else
                Resume();
        }
    }
    public void Pause(){
        Menu.SetActive(true);
        Time.timeScale = 0.0f;
    }
    
    public void Resume(){
        Menu.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void BackToMainMenu(){
        PlayerPrefs.SetInt("Stage",1);
        SceneManager.LoadScene("Scenes/MainMenu");
        Time.timeScale = 1.0f;
    }
}
