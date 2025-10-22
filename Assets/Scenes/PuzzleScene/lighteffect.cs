using UnityEngine;
using UnityEngine.UI;
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
        }
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
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Tạo Image che toàn màn hình
        GameObject fadeObj = new GameObject("WhiteFadeImage");
        fadeObj.transform.SetParent(canvas.transform, false);

        fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 0f);
        fadeImage.raycastTarget = false;

        // Phủ toàn màn hình
        RectTransform rect = fadeObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

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
        {
            audioSource.Play();
        }

        fadeImage.gameObject.SetActive(true);

        // Giai đoạn 1: Lóa lên
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, alpha);
            yield return null;
        }

        // Giai đoạn 2: Giữ sáng
        fadeImage.color = new Color(lightColor.r, lightColor.g, lightColor.b, 1f);
        yield return new WaitForSeconds(holdTime);

        // Giai đoạn 3: Mờ dần
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
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