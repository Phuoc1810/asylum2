using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject settingpannel;
    public GameObject graphicsetting;
    public GameObject audiosetting;
    public GameObject menustting;
    public buttonsetting buttonsettings;
    
    public void option()
    {
        buttonsettings.isopen = true;
        settingpannel.SetActive(true);
        graphicsetting.SetActive(false);
        audiosetting.SetActive(false);
        menustting.SetActive(false);
}
    public void play()
    {
        buttonsettings.count = 1;
        SceneManager.LoadScene(1);
    }
}
