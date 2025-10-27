using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorLightEffect : MonoBehaviour
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    [Header(" Hiệu Ứng Ánh Sáng")]
    [Tooltip("Màu ánh sáng (trắng)")]
=======
    [Header("✨ Hiệu Ứng Ánh Sáng")]
    [Tooltip("Màu ánh sáng (trắng hoặc vàng đều đẹp)")]
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    [Header(" Âm Thanh (Tùy chọn)")]
    [Tooltip("Âm thanh khi mở cửa")]
    public AudioClip doorOpenSound;

    private Image fadeImage;
    private AudioSource audioSource;
=======
=======
>>>>>>> Stashed changes
    [Header("🔊 Âm Thanh (Tùy chọn)")]
    [Tooltip("Âm thanh khi mở cửa")]
    public AudioClip doorOpenSound;

    private GameObject lightFlashUI;
    private Image flashImage;
    private AudioSource audioSource;
    private Animator doorAnimator;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    private bool hasPlayed = false;

    void Start()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // Tự động tạo Canvas và Image
        CreateFadeUI();
=======
        // Tạo UI ánh sáng che toàn màn hình
        CreateLightFlashUI();
>>>>>>> Stashed changes
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
        }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    }

    void CreateFadeUI()
    {
        // Tìm Canvas có sẵn
        Canvas canvas = FindObjectOfType<Canvas>();

        // Nếu không có thì tạo mới
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FadeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999; // Hiển thị trên cùng
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
=======
=======
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Tạo Image che toàn màn hình
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        GameObject fadeObj = new GameObject("WhiteFadeImage");
        fadeObj.transform.SetParent(canvas.transform, false);

        fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);
        fadeImage.raycastTarget = false;

        // Phủ toàn màn hình
        RectTransform rect = fadeObj.GetComponent<RectTransform>();
=======
=======
>>>>>>> Stashed changes
        lightFlashUI = new GameObject("LightFlash");
        lightFlashUI.transform.SetParent(canvas.transform, false);

        flashImage = lightFlashUI.AddComponent<Image>();
        flashImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);

        // Phủ toàn màn hình
        RectTransform rect = lightFlashUI.GetComponent<RectTransform>();
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        fadeObj.SetActive(false);

        Debug.Log(" Fade UI đã được tạo!");
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

        // Phát âm thanh
        if (audioSource != null && doorOpenSound != null)
=======
=======
>>>>>>> Stashed changes
        lightFlashUI.SetActive(false);
    }

    // HÀM NÀY có thể gọi từ bên ngoài (Animation Event hoặc script khác)
    public void TriggerLightEffect()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        // Phát âm thanh
        if (audioSource != null)
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        {
            audioSource.Play();
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        fadeImage.gameObject.SetActive(true);

        // Giai đoạn 1: Lóa lên
=======
=======
>>>>>>> Stashed changes
        // Chạy hiệu ứng ánh sáng
        StartCoroutine(LightFlashEffect());
    }

    IEnumerator LightFlashEffect()
    {
        lightFlashUI.SetActive(true);

        // Giai đoạn 1: Ánh sáng lóa lên nhanh
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        // Giai đoạn 2: Giữ sáng
        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 1f);
        yield return new WaitForSeconds(holdTime);

        // Giai đoạn 3: Mờ dần
=======
=======
>>>>>>> Stashed changes
            flashImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        // Giai đoạn 2: Giữ ánh sáng
        yield return new WaitForSeconds(holdTime);

        // Giai đoạn 3: Ánh sáng mờ dần
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);
        fadeImage.gameObject.SetActive(false);

        Debug.Log(" Hiệu ứng hoàn thành!");
    }

    public void ResetEffect()
    {
        hasPlayed = false;
    }
}
=======
=======
>>>>>>> Stashed changes
            flashImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        lightFlashUI.SetActive(false);
    }
}


<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
