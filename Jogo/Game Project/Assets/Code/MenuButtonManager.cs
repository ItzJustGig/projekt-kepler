using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject loadPanel;
    [SerializeField] Slider slider;

    private SceneLoader loader;

    private void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();
    }

    public void BtnOptions(bool appear)
    {
        if (appear)
        {
            mainPanel.SetActive(false);
            optionsPanel.SetActive(true);
        }
        else
        {
            mainPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    public void BtnRngBattle()
    {
        PlayerPrefs.SetInt("isEndless", 0);

        loader.LoadScene(1, slider, loadPanel);
    }

    public void BtnEndlessBattle()
    {
        EndlessData data = SaveSystem.Load();

        PlayerPrefs.SetInt("isEndless", 1);
        if (data.player.id != -1)
            loader.LoadScene(3, slider, loadPanel);
        else
            loader.LoadScene(1, slider, loadPanel);
    }
    
    public void BtnInfo()
    {
        loader.LoadScene(4, slider, loadPanel);
    }

    public void btnQuit()
    {
        Debug.Log(Application.persistentDataPath + "/endless.data");
	    Application.Quit();
    }
	
}
