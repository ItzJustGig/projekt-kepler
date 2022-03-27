using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(int id, Slider slider, GameObject loadPanel)
    {
        loadPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync(id, slider));
    }

    public IEnumerator LoadSceneAsync(int id, Slider slider)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(id);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slider.value = progress;
            Debug.Log(progress);

            yield return null;
        }

        op.allowSceneActivation = true;
    }
}
