using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject deathCanvas;
    public TextMeshProUGUI terminalText;
    public TextMeshProUGUI headerText;
    public Image terminalPanel;
    public RawImage scanlines;

    [Header("Corner Decorations (Optional)")]
    public TextMeshProUGUI cornerTopLeft;
    public TextMeshProUGUI cornerTopRight;
    public TextMeshProUGUI cornerBottomLeft;
    public TextMeshProUGUI cornerBottomRight;

    [Header("Basic Settings")]
    public float typingSpeed = 0.05f;
    public float lineDelay = 0.3f;
    public KeyCode restartKey = KeyCode.Space;

    [Header("Visual Effects")]
    public bool enableScanlines = true;
    public bool enableFlicker = true;
    public bool enableCRTEffect = true;
    public bool enableGlowPulse = true;

    [Header("Scanline Settings")]
    public float scanlineSpeed = 0.2f;
    public int scanlineCount = 50;

    [Header("Flicker Settings")]
    [Range(0.01f, 0.5f)] public float flickerInterval = 0.1f;
    [Range(0f, 1f)] public float flickerChance = 0.1f;
    [Range(0f, 1f)] public float flickerMinAlpha = 0.8f;

    [Header("CRT Effect Settings")]
    [Range(0f, 0.1f)] public float distortionAmount = 0.02f;
    [Range(0f, 0.2f)] public float distortionChance = 0.05f;
    public bool enableCRTShake = true;
    [Range(0f, 2f)] public float shakeIntensity = 0.5f;
    public bool enableRandomGlitch = true;
    [Range(1f, 10f)] public float glitchInterval = 3f;

    [Header("Glow Pulse Settings")]
    [Range(0.5f, 5f)] public float pulseSpeed = 2f;
    [Range(0f, 1f)] public float glowMinAlpha = 0.5f;
    [Range(0f, 1f)] public float glowMaxAlpha = 1f;

    [Header("Audio (Optional)")]
    public AudioClip deathSound;
    public AudioClip staticSound;
    [Range(0f, 1f)] public float audioVolume = 0.5f;
    [Header("ModelPlayer")]
    public GameObject playerModel;

    private string[] terminalLines = {
        "═══════════════════════════════════════",
        "> HỒ SƠ Y TẾ - BỆNH VIỆN TÂM THẦN ST. MORROW",
        "> Đang truy xuất dữ liệu bác sĩ...",
        "> CẢNH BÁO: Phát hiện dấu hiệu tổn thương nghiêm trọng",
        "> Nhịp tim: 0 bpm... Huyết áp: 0/0...",
        "> TRẠNG THÁI: <color=#FF0000>ĐÃ TỬ VONG</color>",
        "> Nguyên nhân: Tấn công bởi đối tượng thí nghiệm",
        "> ",
        "> Nhấn SPACE để thử lại..."
    };

    // Internal variables
    private bool isTyping = false;
    private bool canRestart = false;
    private bool isLastLine = false;
    private Coroutine cursorBlinkCoroutine;
    private AudioSource audioSource;

    // Scanline variables
    private float scanlineOffset = 0f;
    private Texture2D scanlineTexture;

    // Flicker variables
    private Color originalTextColor;
    private Color originalHeaderColor;

    // CRT variables
    private Vector3 originalPanelPosition;
    private Vector3 originalPanelScale;

    // Glow variables
    private Outline panelOutline;
    private Color glowColor = new Color(0f, 1f, 0.53f, 1f); // #00FF88

    void Start()
    {
        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();

        // Store original values
        if (terminalText != null)
            originalTextColor = terminalText.color;
        if (headerText != null)
            originalHeaderColor = headerText.color;
        if (terminalPanel != null)
        {
            originalPanelPosition = terminalPanel.rectTransform.localPosition;
            originalPanelScale = terminalPanel.rectTransform.localScale;
            panelOutline = terminalPanel.GetComponent<Outline>();
        }

        // Setup scanlines
        if (enableScanlines && scanlines != null)
        {
            SetupScanlines();
        }

        // Setup corner decorations
        SetupCornerDecorations();
    }

    void Update()
    {
        if (canRestart && Input.GetKeyDown(restartKey))
        {
            RestartGame();
        }

        // Update effects only when death screen is active
        if (deathCanvas != null && deathCanvas.activeSelf)
        {
            UpdateScanlines();
            UpdateCRTEffect();
            UpdateGlowPulse();
        }
    }

    public void ShowDeathScreen()
    {
        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);
            StartCoroutine(DeathSequence());
        }
    }

    IEnumerator DeathSequence()
    {
        // Play death sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound, audioVolume);
        }

        // Play static sound loop
        if (staticSound != null && audioSource != null)
        {
            audioSource.loop = true;
            audioSource.clip = staticSound;
            audioSource.volume = audioVolume * 0.3f;
            audioSource.Play();
        }

        yield return new WaitForSeconds(0.3f);

        // Start random glitch if enabled
        if (enableRandomGlitch && enableCRTEffect)
        {
            StartCoroutine(RandomGlitchRoutine());
        }

        // Start typing
        StartCoroutine(TypeAllLines());

        // Start flicker effect
        if (enableFlicker)
        {
            StartCoroutine(FlickerRoutine());
        }
    }

    IEnumerator TypeAllLines()
    {
        isTyping = true;
        canRestart = false;
        isLastLine = false;

        if (terminalText != null)
            terminalText.text = "";

        for (int i = 0; i < terminalLines.Length; i++)
        {
            string line = terminalLines[i];
            isLastLine = (i == terminalLines.Length - 1);

            if (isLastLine && cursorBlinkCoroutine == null)
            {
                cursorBlinkCoroutine = StartCoroutine(BlinkCursor());
            }

            yield return StartCoroutine(TypeLine(line));

            if (terminalText != null)
                terminalText.text += "\n";

            yield return new WaitForSeconds(lineDelay);
        }

        isTyping = false;
        canRestart = true;

        // Stop static sound
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == staticSound)
        {
            audioSource.Stop();
        }
    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            if (terminalText != null)
            {
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

                if (currentText.EndsWith("_"))
                    terminalText.text = currentText.Substring(0, currentText.Length - 1);
                else
                    terminalText.text = currentText + "_";
            }

            yield return new WaitForSeconds(0.5f);
        }

        if (terminalText != null && terminalText.text.EndsWith("_"))
        {
            terminalText.text = terminalText.text.Substring(0, terminalText.text.Length - 1);
        }
    }

    // ========== SCANLINE EFFECT ==========
    void SetupScanlines()
    {
        if (scanlines == null) return;

        // Generate scanline texture
        int width = 2;
        int height = scanlineCount * 2;
        scanlineTexture = new Texture2D(width, height);
        scanlineTexture.wrapMode = TextureWrapMode.Repeat;
        scanlineTexture.filterMode = FilterMode.Point;

        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            Color color = (y % 2 == 0) ? new Color(0, 1, 0, 0.2f) : new Color(0, 0, 0, 0f);
            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = color;
            }
        }

        scanlineTexture.SetPixels(pixels);
        scanlineTexture.Apply();

        scanlines.texture = scanlineTexture;
    }

    void UpdateScanlines()
    {
        if (!enableScanlines || scanlines == null) return;

        scanlineOffset += scanlineSpeed * Time.deltaTime;
        if (scanlineOffset > 1f) scanlineOffset -= 1f;

        Rect uvRect = scanlines.uvRect;
        uvRect.y = scanlineOffset;
        scanlines.uvRect = uvRect;
    }

    // ========== FLICKER EFFECT ==========
    IEnumerator FlickerRoutine()
    {
        while (deathCanvas != null && deathCanvas.activeSelf)
        {
            yield return new WaitForSeconds(flickerInterval);

            if (Random.value < flickerChance)
            {
                // Flicker terminal text
                if (terminalText != null)
                {
                    Color flickerColor = originalTextColor;
                    flickerColor.a = Random.Range(flickerMinAlpha, 1f);
                    terminalText.color = flickerColor;

                    yield return new WaitForSeconds(0.05f);
                    terminalText.color = originalTextColor;
                }
            }
        }
    }

    // ========== CRT EFFECT ==========
    void UpdateCRTEffect()
    {
        if (!enableCRTEffect || terminalPanel == null) return;

        // Continuous shake
        if (enableCRTShake)
        {
            float shake = Mathf.Sin(Time.time * 10f) * shakeIntensity;
            terminalPanel.rectTransform.localPosition = originalPanelPosition + new Vector3(shake, 0f, 0f);
        }

        // Random distortion
        if (Random.value < distortionChance)
        {
            float scaleX = 1f + Random.Range(-distortionAmount, distortionAmount);
            float scaleY = 1f + Random.Range(-distortionAmount, distortionAmount);
            terminalPanel.rectTransform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
        else
        {
            terminalPanel.rectTransform.localScale = originalPanelScale;
        }
    }

    IEnumerator RandomGlitchRoutine()
    {
        while (deathCanvas != null && deathCanvas.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(glitchInterval * 0.5f, glitchInterval * 1.5f));

            if (terminalPanel != null)
            {
                // Strong offset
                terminalPanel.rectTransform.localPosition = originalPanelPosition + new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(-3f, 3f),
                    0f
                );

                yield return new WaitForSeconds(0.05f);

                // Reset
                terminalPanel.rectTransform.localPosition = originalPanelPosition;
            }
        }
    }

    // ========== GLOW PULSE EFFECT ==========
    void UpdateGlowPulse()
    {
        if (!enableGlowPulse || panelOutline == null) return;

        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float alpha = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, t);

        Color color = glowColor;
        color.a = alpha;
        panelOutline.effectColor = color;
    }

    // ========== CORNER DECORATIONS ==========
    void SetupCornerDecorations()
    {
        Color cornerColor = glowColor;

        if (cornerTopLeft != null)
        {
            cornerTopLeft.text = "╔══";
            cornerTopLeft.color = cornerColor;
        }
        if (cornerTopRight != null)
        {
            cornerTopRight.text = "══╗";
            cornerTopRight.color = cornerColor;
        }
        if (cornerBottomLeft != null)
        {
            cornerBottomLeft.text = "╚══";
            cornerBottomLeft.color = cornerColor;
        }
        if (cornerBottomRight != null)
        {
            cornerBottomRight.text = "══╝";
            cornerBottomRight.color = cornerColor;
        }
    }

    // ========== RESTART ==========
    void RestartGame()
    {
        string last_scene_name = this.gameObject.GetComponent<PlayerSaveData>().GetLastSceneName();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(last_scene_name);
        deathCanvas.SetActive(false);
        UnlockPlayerAfterDeath();
        playerModel.SetActive(true);
    }
    public static void UnlockPlayerAfterDeath()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("[Jumpscare] Cannot unlock player - Player object not found!");
            return;
        }

        // Bật lại CharacterController
        CharacterController controller = playerObj.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = true;
            Debug.Log("[Jumpscare] CharacterController enabled");
        }

        // Bật lại các movement scripts
        MonoBehaviour[] scripts = playerObj.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script == null) continue;
            string name = script.GetType().Name.ToLower();
            if (name.Contains("move") || name.Contains("control") || name.Contains("player"))
            {
                script.enabled = true;
                Debug.Log($"[Jumpscare] Re-enabled script: {script.GetType().Name}");
            }
        }

        Debug.Log("[Jumpscare] Player unlocked after death screen");
    }

    public static void TriggerDeath()
    {
        DeathScreenManager manager = FindObjectOfType<DeathScreenManager>();
        if (manager != null)
        {
            manager.ShowDeathScreen();
        }
    }
}