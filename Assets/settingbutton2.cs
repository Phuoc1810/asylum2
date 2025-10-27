using UnityEngine;
using UnityEngine.UI;
public class settingbutton2 : MonoBehaviour
{
    public Button button;
    public GameObject exit;
    public lockmaneger EXIT;
    public string nametag;
    public int count;
    public int dem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        exit = GameObject.FindGameObjectWithTag(nametag);
        if (exit != null)
        {
            EXIT = exit.GetComponent<lockmaneger>();
        }
        if (EXIT != null && dem == 0)
        {
            button.onClick.AddListener(EXIT.close);
            dem += 1;
        }
    }
}
