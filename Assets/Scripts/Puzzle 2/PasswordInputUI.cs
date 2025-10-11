using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordInputUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button[] numberButtons;
    [SerializeField] private Button deleteButton;

    [Header("Controller Reference")]
    [SerializeField] private PasswordLockController lockController;

    [Header("Display Settings")]
    [SerializeField] private int maxLength = 4;
    [SerializeField] private string emptyPlaceholder = "_ _ _ _";

    private string currentInput = "";

    void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int number = i + 1;
            numberButtons[i].onClick.AddListener(() => OnNumberButtonClick(number));
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClick);
        }

        UpdateDisplay();
    }

    private void OnEnable()
    {
        ResetInput();
    }

    private void OnNumberButtonClick(int number)
    {
        if (currentInput.Length >= maxLength) return;

        currentInput += number.ToString();
        UpdateDisplay();

        if (currentInput.Length == maxLength)
        {
            SubmitPassword();
        }
    }

    private void OnDeleteButtonClick()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (displayText == null) return;

        if (currentInput.Length == 0)
        {
            displayText.text = emptyPlaceholder;
        }
        else
        {
            string display = "";
            for (int i = 0; i < maxLength; i++)
            {
                if (i < currentInput.Length)
                {
                    display += currentInput[i];
                }
                else
                {
                    display += "_";
                }

                if (i < maxLength - 1)
                {
                    display += " ";
                }
            }
            displayText.text = display;
        }
    }

    private void SubmitPassword()
    {
        if (lockController != null)
        {
            lockController.CheckPassword(currentInput);
        }
    }

    public void ResetInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    private void OnValidate()
    {
        if (lockController == null)
        {
            lockController = GetComponentInParent<PasswordLockController>();
            if (lockController == null)
            {
                lockController = FindObjectOfType<PasswordLockController>();
            }
        }
    }
}