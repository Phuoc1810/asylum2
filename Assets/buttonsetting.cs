using UnityEngine;

public class buttonsetting : MonoBehaviour
{

    public GameObject graphicsetting;
    public GameObject audiosetting;
    public GameObject optionssetting;
    public GameObject pannel;
    public GameObject menusetting;
    public GameObject setting;
    public bool isopen = false;
    public menu menu;
    public int count=0;
    private void Update()
    {
        if(isopen==false && Input.GetKeyDown(KeyCode.Escape))
        {
            if (count == 0)
            {
                menu.menustting.SetActive(false);
            }
            setting.SetActive(true);
            graphicsetting.SetActive(false);
            optionssetting.SetActive(false);
            audiosetting.SetActive(false);
            isopen = true;
        }
        else if (isopen == true && Input.GetKeyDown(KeyCode.Escape) )
        {
            setting.SetActive(false);
            if (count == 0)
            {
                menu.menustting.SetActive(true);
            }
            isopen =false;
        }

    }
    private void Start()
    {
        isopen = false;
        DontDestroyOnLoad(pannel);
    }
   
    public void graphics()
    {
        graphicsetting.SetActive(true);
        audiosetting.SetActive(false);
        optionssetting.SetActive(false);
    }
    public void options()
    {
        optionssetting.SetActive(true);
        graphicsetting.SetActive(false);
        audiosetting.SetActive(false);
    }
    public void audio()
    {
        graphicsetting.SetActive(false);
        audiosetting.SetActive(true);
        optionssetting.SetActive(false);
    }
    public void exit()
    {
        graphicsetting.SetActive(false);
        audiosetting.SetActive(false);
        optionssetting.SetActive(false);
        setting.SetActive(false);
        if (count == 0)
        {
            menu.menustting.SetActive(true);
        }
    }
}
