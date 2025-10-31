using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class optionsetting : MonoBehaviour
{
    public GameObject menusetting;
    public GameObject panel;
    public GameObject canval;
    public GameObject player;
    public buttonsetting buttonsetting;
    public void backToMEnu()
    {
        
        if (buttonsetting.count == 0)
        {

            menusetting.SetActive(true);
            panel.SetActive(false);
            
        }
        else if (buttonsetting.count == 1)
        {
            if (player != null)
            {
                Destroy(player);
            }
            SceneManager.LoadScene(1);
            Destroy(canval);
        }

    }
    public void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
       
    }
    public void backToWindow()
    {
        Application.Quit();
    }
}
