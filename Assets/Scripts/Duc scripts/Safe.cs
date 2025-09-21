using TMPro;
using UnityEngine;

public class Safe : MonoBehaviour
{
    const int real_password = 1111;
    int input_password = 0;
    int digit1 = 0;
    int digit2 = 0;
    int digit3 = 0;
    int digit4 = 0;
    public TextMeshProUGUI[] digitTXTs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input_password = 0;
        digitTXTs[0].text = digit1.ToString();
        digitTXTs[1].text = digit2.ToString();
        digitTXTs[2].text = digit3.ToString();
        digitTXTs[3].text = digit4.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Change1stDigit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Change2ndDigit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Change3rdDigit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Change4thDigit();
        }
    }

    public void Change1stDigit()
    {
        digit1++;
        if (digit1 > 9)
        {
            digit1 = 0;
        }
        digitTXTs[0].text = digit1.ToString();
        CheckPassword();
    }

    public void Change2ndDigit()
    {
        digit2++;
        if (digit2 > 9)
        {
            digit2 = 0;
        }
        digitTXTs[1].text = digit2.ToString();
        CheckPassword();
    }

    public void Change3rdDigit()
    {
        digit3++;
        if (digit3 > 9)
        {
            digit3 = 0;
        }
        digitTXTs[2].text = digit3.ToString();
        CheckPassword();
    }

    public void Change4thDigit()
    {
        digit4++;
        if (digit4 > 9)
        {
            digit4 = 0;
        }
        digitTXTs[3].text = digit4.ToString();
        CheckPassword();
    }

    void CheckPassword()
    {
        input_password = digit1 * 1000 + digit2 * 100 + digit3 * 10 + digit4;
        if (input_password == real_password)
        {
            Debug.Log("Safe unlocked");
        }
    }
}
