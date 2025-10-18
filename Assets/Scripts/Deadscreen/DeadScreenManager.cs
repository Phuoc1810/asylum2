using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject deathCanvas;
    public TextMeshProUGUI terminalText;
    public TextMeshProUGUI headerText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public float lineDelay = 0.3f;
    public KeyCode restartKey = KeyCode.Space;

    private string[] terminalLines = {
        "> HỒ SƠ Y TẾ - BỆNH VIỆN TÂM THẦN ST. MERCY",
        "> Đang truy xuất dữ liệu bác sĩ...",
        "> CẢNH BÁO: Phát hiện dấu hiệu tổn thương nghiêm trọng",
        "> Nhịp tim: 0 bpm... Huyết áp: 0/0...",
        "> TRẠNG THÁI: ĐÃ TỬ VONG",
        "> Nguyên nhân: Tấn công bởi đối tượng thí nghiệm",
        "> ",
        "> Nhấn SPACE để thử lại..."
    };

    private bool isTyping = false;
    private bool canRestart = false;
    private bool isLastLine = false;
    private Coroutine cursorBlinkCoroutine;

    void Start()
    {
        // Ẩn death screen khi bắt đầu
        if (deathCanvas != null)
            deathCanvas.SetActive(false);
    }

    void Update()
    {
        // Cho phép restart khi hoàn thành typing
        if (canRestart && Input.GetKeyDown(restartKey))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Gọi hàm này khi người chơi chết
    /// </summary>
    public void ShowDeathScreen()
    {
        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);
            StartCoroutine(TypeAllLines());
        }
    }

    IEnumerator TypeAllLines()
    {
        isTyping = true;
        canRestart = false;
        isLastLine = false;

        if (terminalText != null)
            terminalText.text = "";

        // Gõ từng dòng
        for (int i = 0; i < terminalLines.Length; i++)
        {
            string line = terminalLines[i];

            // Kiểm tra xem có phải dòng cuối không
            isLastLine = (i == terminalLines.Length - 1);

            // Bắt đầu nhấp nháy cursor nếu là dòng cuối
            if (isLastLine && cursorBlinkCoroutine == null)
            {
                cursorBlinkCoroutine = StartCoroutine(BlinkCursor());
            }

            yield return StartCoroutine(TypeLine(line));

            // Thêm xuống dòng sau mỗi dòng
            if (terminalText != null)
                terminalText.text += "\n";

            yield return new WaitForSeconds(lineDelay);
        }

        isTyping = false;
        canRestart = true;
    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            if (terminalText != null)
            {
                // Xóa cursor tạm thời khi đang gõ
                string currentText = terminalText.text;
                if (currentText.EndsWith("_"))
                    currentText = currentText.Substring(0, currentText.Length - 1);

                terminalText.text = currentText + c;
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator BlinkCursor()
    {
        while (isLastLine)
        {
            if (terminalText != null)
            {
                string currentText = terminalText.text;

                // Toggle cursor - chỉ ở dòng cuối
                if (currentText.EndsWith("_"))
                    terminalText.text = currentText.Substring(0, currentText.Length - 1);
                else
                    terminalText.text = currentText + "_";
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Xóa cursor khi không còn là dòng cuối
        if (terminalText != null && terminalText.text.EndsWith("_"))
        {
            terminalText.text = terminalText.text.Substring(0, terminalText.text.Length - 1);
        }
    }

    void RestartGame()
    {
        // Tùy chọn 1: Reload scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Tùy chọn 2: Ẩn death screen và reset game
        // deathCanvas.SetActive(false);
        // GameManager.Instance.RestartGame(); // Gọi hàm restart của bạn
    }

    /// <summary>
    /// Gọi từ script khác khi player chết
    /// </summary>
    public static void TriggerDeath()
    {
        DeathScreenManager manager = FindObjectOfType<DeathScreenManager>();
        if (manager != null)
        {
            manager.ShowDeathScreen();
        }
    }
}