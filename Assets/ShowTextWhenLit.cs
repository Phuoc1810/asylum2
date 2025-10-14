using TMPro;
using UnityEngine;

public class ShowTextWhenLit : MonoBehaviour
{
    public Light flashlight;               // Đèn sẽ chiếu (ví dụ đèn pin của người chơi)
    public TextMeshPro textToShow;     // Text UI
    public float lightDetectAngle = 20f;   // Góc chiếu cần để phát hiện
    public float maxDistance = 10f;        // Khoảng cách tối đa để phát hiện

    void Start()
    {
        textToShow.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 dirToText = (textToShow.transform.position - flashlight.transform.position).normalized;
        float angle = Vector3.Angle(flashlight.transform.forward, dirToText);
        float distance = Vector3.Distance(flashlight.transform.position, textToShow.transform.position);

        // Nếu ánh sáng chiếu đúng hướng và trong phạm vi
        bool isLit = angle < lightDetectAngle && distance < maxDistance;
        //aaaaa
        textToShow.gameObject.SetActive(isLit);
    }
}
