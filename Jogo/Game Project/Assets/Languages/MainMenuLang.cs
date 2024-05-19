using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using static LanguageManager;

public class MainMenuLang : MonoBehaviour
{
    [SerializeField] public string language;
    public LanguageManager languageManager;

    [SerializeField] private Sprite flagPt;
    [SerializeField] private Sprite flagEng;
    [SerializeField] private Image flags;

    [SerializeField] private Text fightBtnText;
    [SerializeField] private Text infoBtnText;
    [SerializeField] private Text optionsBtnText;
    [SerializeField] private Text quitBtnText;
    [SerializeField] private Text endlessBtnText;

    [SerializeField] private Text versionText;

    [SerializeField] private Text gameNameText;

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        if (language is "eng")
        {
            flags.sprite = flagEng;
        }
        else
        {
            flags.sprite = flagPt;
        }

        fightBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "fight"));
        infoBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "info"));
        quitBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "quit"));
        optionsBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "options"));
        endlessBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "endless"));
        gameNameText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "text", "game"));
        versionText.text += Application.version;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Lang();
        }
    }

    public void Lang()
    {
        if (language is "eng")
        {
            language = "pt";
        } else
        {
            language = "eng";
            
        }
        PlayerPrefs.SetString("language", language);
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }
}
