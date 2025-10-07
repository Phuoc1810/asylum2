using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light targetLight;
    public float minIntensity = 0f;      // ánh sáng khi tắt
    public float maxIntensity = 3f;      // ánh sáng khi sáng nhất
    public float flickerSpeed = 0.1f;    // tốc độ chớp
    public float blackoutChance = 0.1f;  // tỉ lệ xuất hiện pha tắt hẳn (0.1 = 10%)
    public Vector2 blackoutDuration = new Vector2(0.3f, 1.5f); // khoảng thời gian tắt hẳn (giây)

    private float targetIntensity;
    private float timer;
    private bool isBlackout = false;
    private float blackoutTimer;

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        targetIntensity = targetLight.intensity;
    }

    void Update()
    {
        if (isBlackout)
        {
            blackoutTimer -= Time.deltaTime;
            targetLight.intensity = Mathf.Lerp(targetLight.intensity, 0f, Time.deltaTime * 10f);
            if (blackoutTimer <= 0f)
                isBlackout = false;
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Có xác suất nhỏ để tắt đèn hoàn toàn trong chốc lát
            if (Random.value < blackoutChance)
            {
                isBlackout = true;
                blackoutTimer = Random.Range(blackoutDuration.x, blackoutDuration.y);
            }
            else
            {
                // Thay đổi độ sáng ngẫu nhiên
                targetIntensity = Random.Range(minIntensity, maxIntensity);
                timer = Random.Range(flickerSpeed * 0.5f, flickerSpeed /15f);
            }
        }

        targetLight.intensity = Mathf.Lerp(targetLight.intensity, targetIntensity, Time.deltaTime * 8f);
    }
}
