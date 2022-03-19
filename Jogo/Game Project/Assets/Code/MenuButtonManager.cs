using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject optionsPanel;

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
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void BtnEndlessBattle()
    {
        EndlessData data = SaveSystem.Load();

        PlayerPrefs.SetInt("isEndless", 1);
        if (data.playerId != -1)
            SceneManager.LoadScene(3, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    
    public void BtnInfo()
    {
        SceneManager.LoadScene(4, LoadSceneMode.Single);
    }

    public void btnQuit()
    {
        Debug.Log(Application.persistentDataPath + "/endless.data");
	    Application.Quit();
    }
	
}
