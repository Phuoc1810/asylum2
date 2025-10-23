using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    public GameObject loadingscreen;
    public GameObject menu;
    public Slider loadSlider;
    public buttonsetting buttonsettings;
    // Start is called once b
    // efore the first execution of Update after the MonoBehaviour is created
    public void loadlevelbtn(string levertoload)
    {
        menu.SetActive(false);
        loadingscreen.SetActive(true);
        buttonsettings.count = 1;
        MusicManager.instance.Playmusic("stop");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(loadlevelAsync(levertoload));
    }

    IEnumerator loadlevelAsync(string leveltoload) 
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(leveltoload);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress/0.9f );
            loadSlider.value = progressValue; 
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
