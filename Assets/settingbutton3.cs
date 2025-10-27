using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public class settingbutton3 : MonoBehaviour
{
    public Button button;
    public GameObject Lock;
    public lock3 LOCK;
    public string nametag;
    public string namescript;
    public int count;
    public int dem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Lock = GameObject.FindGameObjectWithTag(nametag);
        if (Lock != null)
        {
            LOCK = Lock.GetComponent<lock3>();
        }
        if (LOCK != null && dem == 0)
        {

            MethodInfo method = LOCK.GetType().GetMethod(namescript, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            button.onClick.AddListener(CallMethodByName);
            dem += 1;
        }
    }
    void CallMethodByName()
    {
        if (LOCK == null || string.IsNullOrEmpty(namescript))
        {
            Debug.LogWarning("Thiếu targetScript hoặc functionName!");
            return;
        }

        // Dùng Reflection để tìm hàm
        MethodInfo method = LOCK.GetType().GetMethod(
           namescript,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (method != null)
        {
            method.Invoke(LOCK, null); // Gọi hàm void không có tham số
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy hàm: {namescript} trong {LOCK.name}");
        }
    }
}
