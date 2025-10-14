using TMPro;
using UnityEngine;

public class ShowTextWhenLit : MonoBehaviour
{
    [Header("Cấu hình đèn và text")]
    public Light flashlight;               // Đèn sẽ chiếu (ví dụ đèn pin của player)
    public TextMeshPro textToShow;         // Text 3D (TextMeshPro - không phải UI)
    public float lightDetectAngle = 25f;   // Góc chiếu hợp lệ (đèn hướng vào text)
    public float maxDistance = 10f;        // Khoảng cách chiếu tối đa

    [Header("Tùy chọn hiển thị")]
    public bool smoothFade = true;         // Làm mờ dần khi sáng/tắt
    public float fadeSpeed = 5f;           // Tốc độ mờ

    private float targetAlpha = 0f;
    private float currentAlpha = 0f;
    private Material textMat;

    void Start()
    {
        if (textToShow == null)
            textToShow = GetComponent<TextMeshPro>();

        textMat = textToShow.fontMaterial;
        SetAlpha(0f); // ẩn hoàn toàn ban đầu
    }

    void Update()
    {
        if (flashlight == null || textToShow == null) return;

        // Tính hướng và khoảng cách giữa đèn và text
        Vector3 dirToText = (transform.position - flashlight.transform.position).normalized;
        float angle = Vector3.Angle(flashlight.transform.forward, dirToText);
        float distance = Vector3.Distance(flashlight.transform.position, transform.position);

        // Kiểm tra điều kiện chiếu sáng

        bool litByLight = IsLitByLight(flashlight, textToShow.transform.position);
        // Chọn alpha mục tiêu
        targetAlpha = litByLight? 1f : 0f;

        // Làm mờ dần nếu cần
        if (smoothFade)
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        else
            currentAlpha = targetAlpha;

        SetAlpha(currentAlpha);
        
    }

    void SetAlpha(float a)
    {
        Color c = textToShow.color;
        c.a = Mathf.Clamp01(a);
        textToShow.color = c;
    }
    bool IsLitByLight(Light light, Vector3 point)
    {
        if (!light.enabled) return false;

        if (light.type == LightType.Point)
        {
            float dist = Vector3.Distance(light.transform.position, point);
            return dist < light.range;
        }
        else if (light.type == LightType.Spot)
        {
            Vector3 toPoint = (point - light.transform.position);
            float dist = toPoint.magnitude;
            float angle = Vector3.Angle(light.transform.forward, toPoint.normalized);
            return (angle < light.spotAngle / 2f) && (dist < light.range);
        }
        else if (light.type == LightType.Directional)
        {
            return true; // ánh sáng mặt trời chiếu khắp nơi
        }
        return false;
    }
}
