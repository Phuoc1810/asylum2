using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class DoorLoadWithFade : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Kéo scene cần load vào đây (Unity Editor only).")]
#if UNITY_EDITOR
    public SceneAsset sceneAsset;   // Cho phép kéo scene
#endif
    [HideInInspector] public string sceneToLoad; // Lưu tên scene thật sự để build

    [Header("Interaction Settings")]
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;
    public bool autoLoadOnEnter = false;

    [Header("Fade Settings")]
    public CanvasGroup fader;
    public float fadeOutTime = 0.5f;
    public float fadeInTime = 0.4f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool keepFaderAcrossScenes = true;

    bool playerInRange = false;
    bool isLoading = false;

    void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Awake() => EnsureFader();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            if (autoLoadOnEnter) StartCoroutine(LoadSceneRoutine());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = false;
    }

    void Update()
    {
        if (isLoading || !playerInRange || autoLoadOnEnter) return;
        if (Input.GetKeyDown(interactKey))
            StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        if (isLoading || string.IsNullOrEmpty(sceneToLoad)) yield break;
        isLoading = true;

        if (fader) yield return FadeTo(1f, fadeOutTime);

        SceneManager.LoadScene(sceneToLoad);

        if (fader) yield return FadeTo(0f, fadeInTime);

        isLoading = false;
    }

    IEnumerator FadeTo(float target, float duration)
    {
        float start = fader.alpha;
        float t = 0f;
        fader.blocksRaycasts = true;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float k = fadeCurve.Evaluate(Mathf.Clamp01(t));
            fader.alpha = Mathf.Lerp(start, target, k);
            yield return null;
        }

        fader.alpha = target;
        fader.blocksRaycasts = target > 0.001f;
    }

    void EnsureFader()
    {
        if (fader != null) return;
        fader = FindObjectOfType<CanvasGroup>(includeInactive: true);
        if (fader != null) return;

        GameObject canvasGO = new GameObject("AutoFadeCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;

        GameObject imgGO = new GameObject("FadeImage", typeof(Image), typeof(CanvasGroup));
        imgGO.transform.SetParent(canvasGO.transform, false);
        imgGO.GetComponent<Image>().color = Color.black;

        RectTransform rt = imgGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        fader = imgGO.GetComponent<CanvasGroup>();
        fader.alpha = 0f;
        fader.blocksRaycasts = false;

        if (keepFaderAcrossScenes)
            DontDestroyOnLoad(canvasGO);
    }
}

#if UNITY_EDITOR
// Custom editor giúp hiển thị và tự động lưu tên scene
[CustomEditor(typeof(DoorLoadWithFade))]
public class DoorLoadWithFadeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DoorLoadWithFade script = (DoorLoadWithFade)target;

        if (script.sceneAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(script.sceneAsset);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            script.sceneToLoad = name;
        }

        if (Application.isPlaying == false)
            EditorUtility.SetDirty(script);
    }
}
#endif
