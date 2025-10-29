using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ending : MonoBehaviour
{
    [Header("UI Targets")]
    [SerializeField] private TMP_Text textUI;    
    [SerializeField] private Image imageUI;       
    [SerializeField] private CanvasGroup group;   

    [Header("Text Style (toàn cục)")]
    [SerializeField] private TMP_FontAsset font;  
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int fontSize = 64;

    [Header("Timing & Transition")]
    [SerializeField, Tooltip("Thời gian mờ hiện mỗi dòng")]
    private float fadeIn = 1.0f;
    [SerializeField, Tooltip("Thời gian mờ tắt mỗi dòng")]
    private float fadeOut = 1.0f;
    [SerializeField, Tooltip("Khoảng nghỉ giữa các dòng sau khi fade out")]
    private float gapAfterFadeOut = 0.2f;
    [SerializeField, Tooltip("Nhân tốc độ tổng thể")]
    private float speed = 1.0f;

    [Header("Playback")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = false;

    [System.Serializable]
    public class OutroItem
    {
        public enum ItemType { Text, Image }

        [Header("Loại nội dung")]
        public ItemType type = ItemType.Text;

        [Header("Nội dung")]
        [TextArea(2, 6)] public string text;

        [Header("Hình")]
        public Sprite sprite;

        [Header("Thời gian giữ")]
        public float holdSeconds = 5f;

        [Header("Ghi chú)")]
        public string note;
    }

    [Header("Danh sách Outtro)")]
    public List<OutroItem> items = new List<OutroItem>()
    {
       
        new OutroItem { type = OutroItem.ItemType.Text, text = "The End", holdSeconds = 5f, note = "Xuất hiện đầu tiên" },
        new OutroItem { type = OutroItem.ItemType.Text, text = "Kịch bản: abcxz", holdSeconds = 5f },
        new OutroItem { type = OutroItem.ItemType.Image, sprite = null, holdSeconds = 5f, note = "Gán sprite vào đây (Hình ảnh: abcxyz)" },
        new OutroItem { type = OutroItem.ItemType.Text, text = "Thank you abcxyz", holdSeconds = 5f }
    };

    private Coroutine _playCo;

    private void Reset()
    {
       
        if (!group) group = GetComponentInParent<CanvasGroup>();
        if (!textUI) textUI = GetComponentInChildren<TMP_Text>(true);
        if (!imageUI) imageUI = GetComponentInChildren<Image>(true);
    }

    private void Awake()
    {
        if (group == null)
        {
            
            GameObject g = (textUI ? textUI.transform.parent.gameObject : gameObject);
            group = g.GetComponent<CanvasGroup>();
            if (!group) group = g.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        ApplyGlobalTextStyle();

       
        SetAlpha(0f);
        ShowText(false);
        ShowImage(false);

        if (playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        if (_playCo != null) StopCoroutine(_playCo);
        _playCo = StartCoroutine(CoPlay());
    }

    public void StopPlayback()
    {
        if (_playCo != null) StopCoroutine(_playCo);
        _playCo = null;
        SetAlpha(0f);
        ShowText(false);
        ShowImage(false);
    }

    private IEnumerator CoPlay()
    {
        do
        {
            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                
                if (it.type == OutroItem.ItemType.Text)
                {
                    if (textUI)
                    {
                        textUI.text = it.text ?? "";
                        ApplyGlobalTextStyle();
                        ShowText(true);
                    }
                    ShowImage(false);
                }
                else 
                {
                    if (imageUI)
                    {
                        imageUI.sprite = it.sprite;
                        imageUI.SetNativeSize(); 
                        imageUI.enabled = (it.sprite != null);
                    }
                    ShowImage(true);
                    ShowText(false);
                }

                
                yield return Fade(0f, 1f, fadeIn / Mathf.Max(0.0001f, speed));

                
                float hold = Mathf.Max(0f, it.holdSeconds) / Mathf.Max(0.0001f, speed);
                yield return new WaitForSeconds(hold);

                yield return Fade(1f, 0f, fadeOut / Mathf.Max(0.0001f, speed));

               
                if (gapAfterFadeOut > 0f)
                    yield return new WaitForSeconds(gapAfterFadeOut / Mathf.Max(0.0001f, speed));
            }
        }
        while (loop);

        
        ShowText(false);
        ShowImage(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            SetAlpha(to);
            yield break;
        }

        float t = 0f;
        SetAlpha(from);
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / duration));
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float a)
    {
        if (group) group.alpha = a;
    }

    private void ShowText(bool on)
    {
        if (textUI) textUI.gameObject.SetActive(on);
    }

    private void ShowImage(bool on)
    {
        if (imageUI) imageUI.gameObject.SetActive(on);
    }

    private void ApplyGlobalTextStyle()
    {
        if (!textUI) return;
        if (font) textUI.font = font;
        textUI.color = textColor;
        if (fontSize > 0) textUI.fontSize = fontSize;
    }

   
    [ContextMenu("Fill Example (Theo yêu cầu)")]
    private void FillExample()
    {
        items = new List<OutroItem>()
        {
            new OutroItem { type = OutroItem.ItemType.Text, text = "The End", holdSeconds = 5f },
            new OutroItem { type = OutroItem.ItemType.Text, text = "Kịch bản: abcxz", holdSeconds = 5f },
            new OutroItem { type = OutroItem.ItemType.Image, sprite = null, holdSeconds = 5f, note = "Gán sprite = abcxyz" },
            new OutroItem { type = OutroItem.ItemType.Text, text = "Thank you abcxyz", holdSeconds = 5f },
        };
    }
}
