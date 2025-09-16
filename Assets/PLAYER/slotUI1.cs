using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTabSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public string name;        // ð? b?n d? nh?n trong Inspector
        public Button button;      // nút/icon c?a tab
        public GameObject content; // panel n?i dung týõng ?ng
    }

    public List<Tab> tabs = new List<Tab>();

    [Header("Hi?u ?ng ch?n")]
    public Color normalColor = new Color(1, 1, 1, 0.35f);
    public Color selectedColor = Color.white;

    private int current = -1;

    void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int idx = i; // capture
            if (tabs[i].button != null)
                tabs[i].button.onClick.AddListener(() => Show(idx));
        }

        // m? tab ð?u tiên khi ch?y
        if (tabs.Count > 0) Show(0);
    }

    void Update()
    {
        // phím t?t 1-9 ð? test nhanh (tùy ch?n)
        if (Input.GetKeyDown(KeyCode.Alpha1)) Show(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Show(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Show(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Show(3);
    }

    public void Show(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        for (int i = 0; i < tabs.Count; i++)
        {
            bool active = (i == index);

            if (tabs[i].content) tabs[i].content.SetActive(active);

            // tô màu icon/nút ð? bi?t tab nào ðang ch?n
            if (tabs[i].button && tabs[i].button.image)
                tabs[i].button.image.color = active ? selectedColor : normalColor;
        }

        current = index;
    }
}
