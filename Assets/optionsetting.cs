using UnityEngine;
using UnityEngine.SceneManagement;

public class optionsetting : MonoBehaviour
{
    public GameObject menusetting;
    public GameObject panel;
    public GameObject canval;
    public buttonsetting buttonsetting;
   public void backToMEnu()
    {
        if (buttonsetting.count == 0)
        {
            panel.SetActive(false);
            menusetting.SetActive(true);
        }
        else if (buttonsetting.count == 1)
        {

            SceneManager.LoadScene(0);
            Destroy(canval);
        }

    }
    public void backToWindow()
    {
        Application.Quit();
    }
}
