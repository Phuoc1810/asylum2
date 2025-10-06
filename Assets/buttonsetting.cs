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
    public GameObject canvas;
    private void Update()
    {
        if (canvas==null)
        {
            canvas = GameObject.FindGameObjectWithTag("canvasplayer");
        }
     
        if(isopen==false && Input.GetKeyDown(KeyCode.Escape))
        {

            canvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
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
            canvas.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
