using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorLightEffect : MonoBehaviour
{
    [Header(" Hiệu Ứng Ánh Sáng")]
    [Tooltip("Màu ánh sáng (trắng hoặc vàng đều đẹp)")]
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

    private GameObject lightFlashUI;
    private Image flashImage;
    private AudioSource audioSource;
    private Animator doorAnimator;
    private bool hasPlayed = false;

    void Start()
    {
        CreateLightFlashUI();
    }

    void Update()
    {
       
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
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        lightFlashUI.SetActive(false);
    }

    // HÀM NÀY có thể gọi từ bên ngoài (Animation Event hoặc script khác)
    public void TriggerLightEffect()
    {
        // Chạy hiệu ứng ánh sáng
        StartCoroutine(LightFlashEffect());
    }

    private IEnumerator LightFlashEffect()
    {
        yield return new WaitForSeconds(4f);
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


