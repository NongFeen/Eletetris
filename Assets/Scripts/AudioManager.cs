using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static AudioManager Instance;
    public AudioSource src;
    public AudioClip[] AudioClips;
    [Range(0f, 1f)] public float sfxVolume;
    public Slider slider;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        sfxVolume = PlayerPrefs.GetFloat("Volume");
        slider.value = sfxVolume;
    }
    // Update is called once per frame
    public void PlaySFX(int clipIndex)
    {
        if(AudioClips[clipIndex] == null){
            Debug.Log("Sound not found");
        }else{
            src.PlayOneShot(AudioClips[clipIndex],sfxVolume);
        }
    }
    public void OnChange(){
        PlayerPrefs.SetFloat("Volume",slider.value);
        sfxVolume = slider.value;
    }
}
