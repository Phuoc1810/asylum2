using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorLightEffect : MonoBehaviour
{
    [Header(" Hiệu Ứng Ánh Sáng")]
    [Tooltip("Màu ánh sáng (trắng)")]
    public Color lightColor = Color.white;

    [Tooltip("Thời gian ánh sáng lóa lên (giây)")]
    public float fadeInTime = 0.3f;

    [Tooltip("Thời gian giữ ánh sáng (giây)")]
    public float holdTime = 0.2f;

    [Tooltip("Thời gian ánh sáng mờ đi (giây)")]
    public float fadeOutTime = 1.5f;

    [Header(" Âm Thanh (Tùy chọn)")]
    [Tooltip("Âm thanh khi mở cửa")]
    public AudioClip doorOpenSound;

    [Header(" Cài Đặt Scene")]
    [Tooltip("Kéo thả scene đích vào đây (phải có trong Build Settings)")]
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset targetScene;
#endif
    [SerializeField, HideInInspector] private string sceneNameRuntime;
    [Tooltip("Độ trễ (giây) trước khi load scene sau khi hiệu ứng xong")]
    public float delayBeforeLoad = 0f;

    private Image fadeImage;
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        // Tự động tạo Canvas và Image
        CreateFadeUI();

        // Setup audio nếu có
        if (doorOpenSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = doorOpenSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    void CreateFadeUI()
    {
        // Tạo Canvas overlay riêng
        GameObject canvasObj = new GameObject("FadeCanvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Image phủ toàn màn hình
        GameObject fadeObj = new GameObject("WhiteFadeImage");
        fadeObj.transform.SetParent(canvas.transform, false);
        fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);
        fadeImage.raycastTarget = false;

        RectTransform rect = fadeObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        fadeObj.SetActive(false);
    }

    public void TriggerLightEffect()
    {
        if (!hasPlayed)
        {
            Debug.Log(" Kích hoạt hiệu ứng ánh sáng!");
            StartCoroutine(LightFlashEffect());
        }
    }

    IEnumerator LightFlashEffect()
    {
        hasPlayed = true;

        // Âm thanh
        if (audioSource != null && doorOpenSound != null)
            audioSource.Play();

        fadeImage.gameObject.SetActive(true);

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        // Giữ sáng
        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 1f);
        yield return new WaitForSecondsRealtime(holdTime);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);
        fadeImage.gameObject.SetActive(false);

        Debug.Log(" Hiệu ứng hoàn thành! Chuẩn bị load scene...");

        yield return new WaitForSecondsRealtime(delayBeforeLoad);
        if (!string.IsNullOrEmpty(sceneNameRuntime))
        {
            SceneManager.LoadScene(sceneNameRuntime);
        }
        else
        {
            Debug.LogError(" Chưa gán scene đích trong DoorLightEffect!");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetScene != null)
            sceneNameRuntime = targetScene.name;
    }
#endif

    public void ResetEffect()
    {
        hasPlayed = false;
    }
}
