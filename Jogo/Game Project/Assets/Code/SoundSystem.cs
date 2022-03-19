using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

public class SoundSystem : MonoBehaviour
{
    [SerializeField] private AudioSource speaker;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite soundSprite;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private GameObject soundMenu;
    
    public void Awake()
    {
        if (PlayerPrefs.HasKey("volume") && PlayerPrefs.HasKey("muted"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            bool muted = Convert.ToBoolean(PlayerPrefs.GetInt("muted"));

            speaker.volume = volume;
            volumeSlider.value = volume;
            speaker.mute = muted;

            if (muted)
            {
                buttonImage.sprite = mutedSprite;
                volumeSlider.interactable = false;
            }
            else
            {
                buttonImage.sprite = soundSprite;
                volumeSlider.interactable = true;
            }
        } 
    }

    public void SetVolume(float volume)
    {
        speaker.volume = volume;
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void Mute()
    {
        if (speaker.mute)
        {
            speaker.mute = false;
            buttonImage.sprite = soundSprite;
            volumeSlider.interactable = true;
            PlayerPrefs.SetInt("muted", 0);
        }
        else
        {
            speaker.mute = true;
            buttonImage.sprite = mutedSprite;
            volumeSlider.interactable = false;
            PlayerPrefs.SetInt("muted", 1);
        }
            
    }

    public void SoundMenu()
    {
        if (soundMenu.activeInHierarchy)
            soundMenu.SetActive(false);
        else
            soundMenu.SetActive(true);    
    }
}
