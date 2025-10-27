using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Lights")]
    [SerializeField] private Light lowLight;
    [SerializeField] private Light highLight;
    [SerializeField] private Light uvLight;

    [Header("Battery Setting")]
    [SerializeField] private float lowModeDuration = 360f;
    [SerializeField] private float highModeDuration = 240f;
    [SerializeField] private float uvModeDuration = 120f;

    [Header("UI")]
    [SerializeField] private GameObject batteryUI;
    [SerializeField] private TextMeshProUGUI batteryText;

    [Header("Audio")]
    [SerializeField] private AudioClip toggleSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Low Battery Effect")]
    [SerializeField] private float lowBatteryThreashold = 10f;
    [SerializeField] private float flickerSpeed = 0.2f;

    private enum FlashlightMode
    {
        Off,
        Low,
        High,
        UV
    }

    private FlashlightMode currentMode = FlashlightMode.Off;

    private float lowModeTimeLeft;
    private float highModeTimeLeft;
    private float uvModeTimeLeft;

    private bool isFlickering = false;
    private Coroutine flickerCoroutine;

    public static FlashlightController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetBattery();
    }
    private void Start()
    {
        InitializeFlashlight();
    }
    private void Update()
    {
        HandleFlashlighInput();
        UpdateBatteryDrain();
    }
    private void InitializeFlashlight()
    {
        SetAllLightsOff();

        if (batteryUI != null)
            batteryUI.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource.gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }
    private void HandleFlashlighInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && HasFlashlight())
        {
            CycleFlashlightMode();
        }
    }
    private void CycleFlashlightMode()
    {
        StopFlickering();

        switch (currentMode)
        {
            case FlashlightMode.Off:
                if (lowModeTimeLeft > 0)
                    SetMode(FlashlightMode.Low);
                break;
            case FlashlightMode.Low:
                if (highModeTimeLeft > 0)
                    SetMode(FlashlightMode.High);
                else
                    SetMode(FlashlightMode.Off);
                break;
            case FlashlightMode.High:
                if (uvModeTimeLeft > 0)
                    SetMode(FlashlightMode.UV);
                else
                    SetMode(FlashlightMode.Off);
                break;
            case FlashlightMode.UV:
                SetMode(FlashlightMode.Off);
                break;
        }
        PlayToggleSound();
    }
    private void SetMode(FlashlightMode mode)
    {
        currentMode = mode;
        SetAllLightsOff();

        switch (mode)
        {
            case FlashlightMode.Off:
                if (batteryUI != null)
                {
                    batteryUI.SetActive(false);
                }
                break;

            case FlashlightMode.Low:
                if (lowLight != null)
                {
                    lowLight.enabled = true;
                }
                if (batteryUI != null)
                {
                    batteryUI.SetActive(true);
                }
                break;

            case FlashlightMode.High:
                if (highLight != null)
                    highLight.enabled = true;
                if (batteryUI != null)
                    batteryUI.SetActive(true);
                break;

            case FlashlightMode.UV:
                if (uvLight != null)
                    uvLight.enabled = true;
                if (batteryUI != null)
                    batteryUI.SetActive(true);
                break;
        }
        UpdateBatteryUI();
    }
    private void SetAllLightsOff()
    {
        if (lowLight != null) lowLight.enabled = false;
        if (highLight != null) highLight.enabled = false;
        if (uvLight != null) uvLight.enabled = false;
    }
    private void UpdateBatteryDrain()
    {
        if (currentMode == FlashlightMode.Off) return;

        float drainAmount = Time.deltaTime;
        bool outOfBattery = false;

        switch (currentMode)
        {
            case FlashlightMode.Low:
                lowModeTimeLeft -= drainAmount;
                if (lowModeTimeLeft <= 0)
                {
                    lowModeTimeLeft = 0;
                    outOfBattery = true;
                }
                break;
            case FlashlightMode.High:
                highModeTimeLeft -= drainAmount;
                if (highModeTimeLeft <= 0)
                {
                    highModeTimeLeft = 0;
                    outOfBattery = true;
                }
                break;
            case FlashlightMode.UV:
                uvModeTimeLeft -= drainAmount;
                if (uvModeTimeLeft <= 0)
                {
                    uvModeTimeLeft = 0;
                    outOfBattery = true;
                }
                break;
        }
        UpdateBatteryUI();

        float currentPercentage = GetCurrentBatteryParcentage();
        if (currentPercentage <= lowBatteryThreashold && currentPercentage > 0 && !isFlickering)
        {
            StartFlickering();
        }
        if (outOfBattery)
        {
            HandleBatteryDead();
        }
    }
    private void UpdateBatteryUI()
    {
        if (batteryText == null || currentMode == FlashlightMode.Off) return;

        float percentage = GetCurrentBatteryParcentage();
        batteryText.text = $"{Mathf.CeilToInt(percentage)}%";

        if (percentage <= lowBatteryThreashold)
        {
            batteryText.color = Color.red;
        }
        else if (percentage <= 30f)
        {
            batteryText.color = Color.yellow;
        }
        else
        {
            batteryText.color = Color.white;
        }
    }
    private float GetCurrentBatteryParcentage()
    {
        switch (currentMode)
        {
            case FlashlightMode.Low:
                return (lowModeTimeLeft / lowModeDuration) * 100f;
            case FlashlightMode.High:
                return (highModeTimeLeft / highModeDuration) * 100f;
            case FlashlightMode.UV:
                return (uvModeTimeLeft / uvModeDuration) * 100f;
            default:
                return 0f;
        }
    }
    private void HandleBatteryDead()
    {
        if (!isFlickering)
        {
            StartCoroutine(DeathFlickerSequence());
        }
    }
    private IEnumerator DeathFlickerSequence()
    {
        isFlickering = true;

        for(int i=0; i < 5; i++)
        {
            SetCurrentLightActive(false);
            yield return new WaitForSeconds(0.1f);
            SetCurrentLightActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        SetMode(FlashlightMode.Off);
        isFlickering = false;

        PlayToggleSound();
    }
    private void StartFlickering()
    {
        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);

        flickerCoroutine = StartCoroutine(FlickerEffect());
    }
    private void StopFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
        isFlickering = false;
    }
    private IEnumerator FlickerEffect()
    {
        isFlickering = true;

        while (isFlickering && GetCurrentBatteryParcentage() > 0)
        {
            SetCurrentLightActive(false);
            yield return new WaitForSeconds(flickerSpeed);
            SetCurrentLightActive(true);
            yield return new WaitForSeconds(flickerSpeed * 2f);
        }

        isFlickering = false;
    }
    private void SetCurrentLightActive(bool active)
    {
        switch (currentMode)
        {
            case FlashlightMode.Low:
                if (lowLight != null) lowLight.enabled = active;
                break;
            case FlashlightMode.High:
                if (highLight != null) highLight.enabled = active;
                break;
            case FlashlightMode.UV:
                if (uvLight != null) uvLight.enabled = active;
                break;
        }
    }
    private void PlayToggleSound()
    {
        if (audioSource != null && toggleSound != null)
        {
            audioSource.PlayOneShot(toggleSound);
        }
    }
    private bool HasFlashlight()
    {
        return InventoryService.Instance != null &&
            InventoryService.Instance.Contains("flashlight");
    }
    public void ResetBattery()
    {
        lowModeTimeLeft = lowModeDuration;
        highModeTimeLeft = highModeDuration;
        uvModeTimeLeft = uvModeDuration;

        StopFlickering();
        UpdateBatteryUI();
    }
    public bool IsUVModeActive()
    {
        return currentMode == FlashlightMode.UV && uvLight != null && uvLight.enabled;
    }
}
