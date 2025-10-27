using UnityEngine;
using UnityEngine.UI;

public class settingbutton : MonoBehaviour
{
    public Button button;
    public GameObject door3;
    public DOOR3 dOOR3;
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
        door3 = GameObject.FindGameObjectWithTag(nametag).transform.GetChild(count)?.gameObject;
        if (door3 != null)
        {
            dOOR3 = door3.GetComponent<DOOR3>();
        }
        if (dOOR3 != null && dem==0)
        {
            button.onClick.AddListener(dOOR3.locknumber);
            dem += 1;
        }
    }
}
