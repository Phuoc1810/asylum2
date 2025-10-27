using UnityEngine;
using UnityEngine.UI;
<<<<<<< Updated upstream
using UnityEngine.SceneManagement;
=======
>>>>>>> Stashed changes
using System.Collections;

public class DoorLightEffect : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header(" Hiệu Ứng Ánh Sáng")]
    [Tooltip("Màu ánh sáng (trắng)")]
=======
    [Header("✨ Hiệu Ứng Ánh Sáng")]
    [Tooltip("Màu ánh sáng (trắng hoặc vàng đều đẹp)")]
>>>>>>> Stashed changes
    public Color lightColor = Color.white;

    [Tooltip("Thời gian ánh sáng lóa lên (giây)")]
    public float fadeInTime = 0.3f;

    [Tooltip("Thời gian giữ ánh sáng (giây)")]
    public float holdTime = 0.2f;

    [Tooltip("Thời gian ánh sáng mờ đi (giây)")]
    public float fadeOutTime = 1.5f;

<<<<<<< Updated upstream
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
=======
    [Header("🔊 Âm Thanh (Tùy chọn)")]
    [Tooltip("Âm thanh khi mở cửa")]
    public AudioClip doorOpenSound;

    private GameObject lightFlashUI;
    private Image flashImage;
    private AudioSource audioSource;
    private Animator doorAnimator;
>>>>>>> Stashed changes
    private bool hasPlayed = false;

    void Start()
    {
<<<<<<< Updated upstream
        // Tự động tạo Canvas và Image
        CreateFadeUI();
=======
        // Tạo UI ánh sáng che toàn màn hình
        CreateLightFlashUI();
>>>>>>> Stashed changes

        // Setup audio nếu có
        if (doorOpenSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = doorOpenSound;
            audioSource.playOnAwake = false;
<<<<<<< Updated upstream
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
=======
        }

        // Tìm Animator trên cửa
        doorAnimator = GetComponent<Animator>();
        if (doorAnimator == null)
        {
            Debug.LogWarning("Không tìm thấy Animator! Gắn Animator vào cửa để hiệu ứng hoạt động.");
        }
    }

    void Update()
    {
        // Kiểm tra xem animation cửa có đang chạy không
        if (doorAnimator != null && !hasPlayed)
        {
            AnimatorStateInfo stateInfo = doorAnimator.GetCurrentAnimatorStateInfo(0);

            // Nếu đang chạy bất kỳ animation nào (cửa đang mở)
            if (stateInfo.normalizedTime > 0f && stateInfo.normalizedTime < 0.3f)
            {
                TriggerLightEffect();
            }
        }
    }

    void CreateLightFlashUI()
    {
        // Tìm hoặc tạo Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("DoorLightCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Tạo Image che toàn màn hình
        lightFlashUI = new GameObject("LightFlash");
        lightFlashUI.transform.SetParent(canvas.transform, false);

        flashImage = lightFlashUI.AddComponent<Image>();
        flashImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);

        // Phủ toàn màn hình
        RectTransform rect = lightFlashUI.GetComponent<RectTransform>();
>>>>>>> Stashed changes
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

<<<<<<< Updated upstream
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
=======
        lightFlashUI.SetActive(false);
    }

    // HÀM NÀY có thể gọi từ bên ngoài (Animation Event hoặc script khác)
    public void TriggerLightEffect()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        // Phát âm thanh
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Chạy hiệu ứng ánh sáng
        StartCoroutine(LightFlashEffect());
    }

    IEnumerator LightFlashEffect()
    {
        lightFlashUI.SetActive(true);

        // Giai đoạn 1: Ánh sáng lóa lên nhanh
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            flashImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        // Giai đoạn 2: Giữ ánh sáng
        yield return new WaitForSeconds(holdTime);

        // Giai đoạn 3: Ánh sáng mờ dần
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            flashImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        lightFlashUI.SetActive(false);
    }
}


>>>>>>> Stashed changes
