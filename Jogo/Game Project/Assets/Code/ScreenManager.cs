using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public Dropdown resDrop;
    public Toggle fullscrnToggle;
    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        resDrop.ClearOptions();

        List<string> resString = new List<string>();
        int curRes = 0;

        for (int i=0; i<resolutions.Length; i++)
        {
            string res = resolutions[i].width + " x " + resolutions[i].height;
            resString.Add(res);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                curRes = i;
            }
        }

        resDrop.AddOptions(resString);
        resDrop.value = curRes;
        resDrop.RefreshShownValue();

        SetRes();
        fullscrnToggle.isOn = Screen.fullScreen;
    }

    public void SetRes(int resIndex)
    {
        Resolution res = resolutions[resIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetRes()
    {
        Resolution res = Screen.currentResolution;
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscrn)
    {
        Screen.fullScreen = isFullscrn;
    }

}
