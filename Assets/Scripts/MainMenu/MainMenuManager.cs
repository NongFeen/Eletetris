using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManuManager : MonoBehaviour
{
    public void OnSelectClass(int type){
        PlayerPrefs.SetInt("Class",(int)type);
        LoadScene();
    }
    public void LoadScene(){
        SceneManager.LoadScene("Scenes/GameScene");
    }
    public void ExitGame(){
        Application.Quit();
    }
}
