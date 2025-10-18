using TMPro;
using UnityEngine;
public class SafeUnlocker : MonoBehaviour
{
    public GameObject SafeTextUI;
    public GameObject SafeUI;
    bool isOpen;
    bool isInteracted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SafeTextUI.SetActive(false);
        SafeUI.SetActive(false);
        isOpen = false;
        isInteracted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteracted && Input.GetKeyDown(KeyCode.P))
        {
            if (!isOpen)
            {
                SafeUI.SetActive(true);
                SafeTextUI.SetActive(false);
                isOpen = true;
            }
            else
            {
                SafeUI.SetActive(false);
                SafeTextUI.SetActive(true);
                isOpen = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "safe")
        {
            SafeTextUI.SetActive(true);
            isInteracted = true;
            Debug.Log("Open safe");
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.tag == "safe" && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P pressed");
            if (!isOpen)
            {
                SafeUI.SetActive(true);
                SafeTextUI.SetActive(false);
                isOpen = true;
            }
            else
            {
                SafeUI.SetActive(false);
                SafeTextUI.SetActive(true);
                isOpen = false;
            }
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "safe")
        {
            isInteracted = false;
            SafeTextUI.SetActive(false);
        }
    }
}
