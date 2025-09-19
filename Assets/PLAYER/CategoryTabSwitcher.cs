using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTabSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public string name;           // tên hi?n th? (tu? ý)
        public Button button;         // nút/icon
        public GameObject content;    // panel n?i dung
    }

    public List<Tab> tabs = new List<Tab>();

    [Header("Màu icon")]
    public Color normalColor = new Color(1, 1, 1, 0.35f);
    public Color selectedColor = Color.white;

    int current = -1;

    void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int idx = i; // capture bi?n
            if (tabs[i].button != null)
                tabs[i].button.onClick.AddListener(() => Show(idx));
        }

        if (tabs.Count > 0) Show(0); // m? tab ??u
    }

    public void Show(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        for (int i = 0; i < tabs.Count; i++)
        {
            bool active = (i == index);

            if (tabs[i].content)
                tabs[i].content.SetActive(active);

            if (tabs[i].button)
            {
                var img = tabs[i].button.GetComponent<Image>();
                if (img) img.color = active ? selectedColor : normalColor;
            }
        }

        current = index;
    }
}
