using UnityEngine;
using UnityEngine.UI;
public class settingbutton5 : MonoBehaviour
{
    public Button button;
    public GameObject lock3;
    public lock4 LOCK3;
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
        lock3 = GameObject.FindGameObjectWithTag(nametag).transform.GetChild(count)?.gameObject;
        if (lock3 != null)
        {
            LOCK3 = lock3.GetComponent<lock4>();

        }
        if (LOCK3 != null && dem == 0)
        {
            button.onClick.AddListener(LOCK3.locknumber);
            dem += 1;
        }
    }
}
