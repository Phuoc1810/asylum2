using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingSequencer : MonoBehaviour
{
    [Header("UI (để trống sẽ tự tạo)")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI label;

    [Header("Kiểu chữ")]
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private float fontSize = 80f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Center;

    [Header("Nội dung")]
    [TextArea] public string theEndText = "THE END";
    [TextArea] public string[] credits = { "Được thực hiện bởi …", "Lập trình: …", "Thiết kế: …" };
    [TextArea] public string finalText = "Thanks for playing";

    [Header("Thời gian (giây)")]
    // THE END: xuất hiện 0s, giữ 5s, biến mất
    public float theEndFadeIn = 0f;
    public float theEndHold = 5f;
    public float theEndFadeOut = 0f;     // 0 = biến mất ngay

    // Credit: xuất hiện 5s, giữ 5s, biến mất
    public float creditFadeIn = 5f;
    public float creditHold = 5f;
    public float creditFadeOut = 0f;     // 0 = biến mất ngay

    // Cuối cùng: “Thanks for playing”
    public float finalFadeIn = 5f;
    public bool keepFinalOnScreen = true; // giữ lại không tắt
    public float finalHold = 5f;        // dùng nếu không giữ lại
    public float finalFadeOut = 0f;

    [Range(0f, 1f)] public float smooth = 0.8f; // độ mượt (0=linear, 1=smooth)

    void Awake()
    {
        SetupUI();
        StartCoroutine(PlaySequence());
    }

    void SetupUI()
    {
        if (canvas == null)
        {
            var go = new GameObject("EndingCanvas");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            go.AddComponent<GraphicRaycaster>();
            group = go.AddComponent<CanvasGroup>();
        }
        if (group == null) group = canvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        if (label == null)
        {
            var t = new GameObject("EndingLabel");
            t.transform.SetParent(canvas.transform, false);
            label = t.AddComponent<TextMeshProUGUI>();
            var rt = label.rectTransform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        if (font != null) label.font = font;
        label.fontSize = fontSize;
        label.color = textColor;
        label.alignment = alignment;
        label.raycastTarget = false;
    }

    IEnumerator PlaySequence()
    {
        // 1) THE END (0s xuất hiện, giữ 5s, biến mất)
        yield return ShowOne(theEndText, theEndFadeIn, theEndHold, theEndFadeOut);

        // 2) Credits (mỗi dòng: 5s hiện, 5s giữ, biến mất)
        if (credits != null)
        {
            foreach (var c in credits)
                yield return ShowOne(c, creditFadeIn, creditHold, creditFadeOut);
        }

        // 3) Final line
        if (keepFinalOnScreen)
        {
            // hiện dần rồi giữ luôn
            yield return Fade(0f, 1f, finalFadeIn);
            label.text = finalText; // đổi text tại đầu fade-in
            // sửa nhỏ: đổi text trước rồi fade
        }
        else
        {
            yield return ShowOne(finalText, finalFadeIn, finalHold, finalFadeOut);
        }
    }

    IEnumerator ShowOne(string text, float fadeIn, float hold, float fadeOut)
    {
        label.text = text;
        group.alpha = 0f;

        if (fadeIn > 0f) yield return Fade(0f, 1f, fadeIn);
        else group.alpha = 1f;

        if (hold > 0f) yield return new WaitForSecondsRealtime(hold);

        if (fadeOut > 0f) yield return Fade(1f, 0f, fadeOut);
        else group.alpha = 0f;
    }

    IEnumerator Fade(float from, float to, float dur)
    {
        float t = 0f;
        group.alpha = from;
        if (dur <= 0f) { group.alpha = to; yield break; }

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float x = Mathf.Clamp01(t / dur);
            float eased = Mathf.Lerp(x, Mathf.SmoothStep(0f, 1f, x), smooth);
            group.alpha = Mathf.Lerp(from, to, eased);
            yield return null;
        }
        group.alpha = to;
    }
}